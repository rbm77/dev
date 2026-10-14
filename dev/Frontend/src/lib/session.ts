import "server-only";

import { cache } from "react";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { decodeToken, isExpired, type TokenIdentity } from "@/lib/jwt";
import { SESSION_COOKIE, SESSION_COOKIE_OPTIONS } from "@/lib/session-cookie";

type SessionState =
  | { status: "none" }
  | { status: "invalid" }
  | { status: "valid"; token: string; identity: TokenIdentity };

// Reads and decodes the session cookie once per render pass.
const readSession = cache(async (): Promise<SessionState> => {
  const token = (await cookies()).get(SESSION_COOKIE)?.value;
  if (!token) {
    return { status: "none" };
  }

  const identity = decodeToken(token);
  if (!identity || isExpired(identity)) {
    return { status: "invalid" };
  }

  return { status: "valid", token, identity };
});

/** Current user, or null when there is no valid session. Never includes the token. */
export async function getSession(): Promise<TokenIdentity | null> {
  const session = await readSession();
  return session.status === "valid" ? session.identity : null;
}

/**
 * Data Access Layer guard: returns the current user or redirects to sign in.
 * Call it in every page, Server Action and Route Handler that needs a user.
 */
export async function verifySession(): Promise<TokenIdentity> {
  const session = await readSession();
  if (session.status === "none") {
    redirect("/login");
  }
  if (session.status === "invalid") {
    redirect("/session-expired");
  }
  return session.identity;
}

/** Bearer token for API calls. Only lib/api.ts should use this. */
export async function getAccessToken(): Promise<string> {
  const session = await readSession();
  if (session.status !== "valid") {
    await verifySession();
    throw new Error("Unreachable: verifySession redirects without a valid session.");
  }
  return session.token;
}

/** Stores the backend token in the httpOnly cookie. Server Actions / Route Handlers only. */
export async function createSession(token: string): Promise<boolean> {
  const identity = decodeToken(token);
  if (!identity || isExpired(identity)) {
    return false;
  }

  const maxAge = Math.max(1, identity.expiresAt - Math.floor(Date.now() / 1000));
  (await cookies()).set(SESSION_COOKIE, token, { ...SESSION_COOKIE_OPTIONS, maxAge });
  return true;
}

/** Removes the session cookie. Server Actions / Route Handlers only. */
export async function deleteSession(): Promise<void> {
  // Setting an expired cookie (instead of delete) keeps the Secure attribute,
  // which browsers require to clear a `__Host-` cookie.
  (await cookies()).set(SESSION_COOKIE, "", { ...SESSION_COOKIE_OPTIONS, maxAge: 0 });
}
