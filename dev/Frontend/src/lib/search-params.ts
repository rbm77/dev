import { isIsoDate } from "@/lib/dates";
import { parseId } from "@/lib/validation";

// Filters, search, tabs and pagination live in the URL. Everything read from it
// is untrusted, so each helper validates and normalizes its value.

export type SearchParams = Record<string, string | string[] | undefined>;

export const PAGE_SIZE = 20;

/** First value of a query param, trimmed and length-limited. */
export function getParam(params: SearchParams, key: string, maxLength = 200): string | undefined {
  const raw = params[key];
  const value = (Array.isArray(raw) ? raw[0] : raw)?.trim();
  return value ? value.slice(0, maxLength) : undefined;
}

export function getPage(params: SearchParams): number {
  const page = Number(getParam(params, "page"));
  return Number.isSafeInteger(page) && page >= 1 ? page : 1;
}

export function getIdParam(params: SearchParams, key: string): number | undefined {
  return parseId(getParam(params, key)) ?? undefined;
}

export function getBoolParam(params: SearchParams, key: string): boolean | undefined {
  const value = getParam(params, key);
  return value === "true" ? true : value === "false" ? false : undefined;
}

export function getDateParam(params: SearchParams, key: string): string | undefined {
  const value = getParam(params, key);
  return value && isIsoDate(value) ? value : undefined;
}

/** Allowed value of an enum-like param (e.g. tabs). */
export function getOptionParam<T extends string>(params: SearchParams, key: string, options: readonly T[]): T | undefined {
  const value = getParam(params, key);
  return options.find((option) => option === value);
}

/** True when any of the given filter params has a value. */
export function hasFilters(params: SearchParams, keys: readonly string[]): boolean {
  return keys.some((key) => getParam(params, key) !== undefined);
}

/** Stable key of the current params, used to restart list Suspense boundaries. */
export function paramsKey(params: SearchParams): string {
  return JSON.stringify(Object.entries(params).sort(([a], [b]) => a.localeCompare(b)));
}

/** Builds an href keeping the current params and applying `changes` (undefined removes a key). */
export function hrefWith(
  pathname: string,
  params: SearchParams,
  changes: Record<string, string | number | undefined>,
): string {
  const query = new URLSearchParams();
  for (const [key, raw] of Object.entries(params)) {
    const value = Array.isArray(raw) ? raw[0] : raw;
    if (value) {
      query.set(key, value);
    }
  }
  for (const [key, value] of Object.entries(changes)) {
    if (value === undefined || value === "") {
      query.delete(key);
    } else {
      query.set(key, String(value));
    }
  }
  const search = query.toString();
  return search ? `${pathname}?${search}` : pathname;
}
