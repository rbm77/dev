import "server-only";

import { redirect } from "next/navigation";
import { messageForStatus } from "@/lib/errors";
import { getAccessToken } from "@/lib/session";

const REQUEST_TIMEOUT_MS = 15_000;

// Only plain path segments are allowed: ids must be validated before they are
// interpolated, and this guard blocks traversal or host injection regardless.
const SAFE_PATH = /^(\/[A-Za-z0-9-]+)+$/;

/** Generic paged response of the API (Common/Shared/PagedResult.cs). */
export type Paged<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
};

export type QueryValue = string | number | boolean | null | undefined;
export type Query = Readonly<Record<string, QueryValue>>;

/** Failed API call. `message` is already safe to show to the user. */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

type RequestOptions = {
  query?: Query;
  body?: unknown;
  /** Send the session token. False only for anonymous endpoints (login, password reset). */
  auth?: boolean;
};

function buildUrl(path: string, query?: Query): URL {
  const baseUrl = process.env.API_BASE_URL;
  if (!baseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }
  if (!SAFE_PATH.test(path)) {
    throw new Error(`Refusing to call an unsafe API path: ${path}`);
  }

  const url = new URL(baseUrl.replace(/\/+$/, "") + path);
  for (const [key, value] of Object.entries(query ?? {})) {
    if (value !== undefined && value !== null && value !== "") {
      url.searchParams.set(key, String(value));
    }
  }
  return url;
}

// Reads the backend error text for server logs only. Middleware errors use
// `Message` (PascalCase) while controllers use `message`; ProblemDetails has `title`.
async function readErrorDetail(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as Record<string, unknown>;
    const detail = body.message ?? body.Message ?? body.title;
    return typeof detail === "string" ? detail : "";
  } catch {
    return "";
  }
}

async function send(method: string, path: string, options: RequestOptions = {}): Promise<Response> {
  const { query, body, auth = true } = options;
  const headers = new Headers({ Accept: "application/json" });
  if (auth) {
    headers.set("Authorization", `Bearer ${await getAccessToken()}`);
  }
  if (body !== undefined) {
    headers.set("Content-Type", "application/json");
  }

  let response: Response;
  try {
    response = await fetch(buildUrl(path, query), {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      cache: "no-store",
      // Never follow redirects: the bearer token must not travel to another URL.
      redirect: "manual",
      signal: AbortSignal.timeout(REQUEST_TIMEOUT_MS),
    });
  } catch (error) {
    console.error(`[api] ${method} ${path} failed: ${error instanceof Error ? error.message : String(error)}`);
    throw new ApiError(0, messageForStatus(0));
  }

  if (response.status === 401 && auth) {
    redirect("/session-expired");
  }

  if (!response.ok) {
    const detail = await readErrorDetail(response);
    console.error(`[api] ${method} ${path} -> ${response.status}${detail ? ` (${detail})` : ""}`);
    throw new ApiError(response.status, messageForStatus(response.status));
  }

  return response;
}

/** Extracts the new id from the `Location` header of a 201 Created response. */
function parseCreatedId(response: Response): number | null {
  const location = response.headers.get("Location");
  if (!location) {
    return null;
  }
  const lastSegment = new URL(location, "http://localhost").pathname.split("/").filter(Boolean).pop();
  const id = Number(lastSegment);
  return Number.isSafeInteger(id) && id > 0 ? id : null;
}

/** GET that returns JSON. */
export async function apiGet<T>(path: string, query?: Query): Promise<T> {
  const response = await send("GET", path, { query });
  return (await response.json()) as T;
}

/** GET of a single resource; returns null when the API answers 404. */
export async function apiGetOptional<T>(path: string): Promise<T | null> {
  try {
    return await apiGet<T>(path);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      return null;
    }
    throw error;
  }
}

/** POST that returns a JSON body (e.g. authenticate, QR codes). */
export async function apiPostJson<T>(path: string, body: unknown, options: { auth?: boolean } = {}): Promise<T> {
  const response = await send("POST", path, { body, auth: options.auth });
  return (await response.json()) as T;
}

/** POST that creates a resource (201 + Location). Returns the new id when the API exposes it. */
export async function apiCreate(path: string, body: unknown): Promise<number | null> {
  const response = await send("POST", path, { body });
  return parseCreatedId(response);
}

/** Request without a response body (204, 202, or 201 whose id is not needed). */
export async function apiSend(
  method: "POST" | "PUT" | "DELETE",
  path: string,
  body?: unknown,
  options: { auth?: boolean } = {},
): Promise<void> {
  await send(method, path, { body, auth: options.auth });
}
