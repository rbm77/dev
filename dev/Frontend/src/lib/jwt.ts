import { parsePermissionCodes, type Permission } from "@/lib/permissions";

/**
 * Identity carried by the backend JWT (Features/Users/TokenHandler.cs).
 * JwtSecurityTokenHandler maps ClaimTypes.NameIdentifier to `nameid` and
 * ClaimTypes.Name to `unique_name`; `cid` and `permissions` are custom claims.
 */
export type TokenIdentity = {
  userId: number;
  companyId: number;
  username: string;
  permissions: Permission[];
  /** Expiration as a Unix timestamp in seconds. */
  expiresAt: number;
};

function decodeBase64Url(segment: string): string {
  const base64 = segment.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  const bytes = Uint8Array.from(atob(padded), (char) => char.charCodeAt(0));
  return new TextDecoder().decode(bytes);
}

function toPositiveInt(value: unknown): number | null {
  const number = typeof value === "string" ? Number(value) : value;
  return typeof number === "number" && Number.isSafeInteger(number) && number > 0 ? number : null;
}

/**
 * Reads the token payload WITHOUT verifying its signature. The API verifies the
 * signature on every request; this is only used to know who is signed in and
 * which UI to show. Returns null for malformed tokens.
 */
export function decodeToken(token: string): TokenIdentity | null {
  const parts = token.split(".");
  if (parts.length !== 3) {
    return null;
  }

  try {
    const payload: unknown = JSON.parse(decodeBase64Url(parts[1]));
    if (typeof payload !== "object" || payload === null) {
      return null;
    }

    const claims = payload as Record<string, unknown>;
    const userId = toPositiveInt(claims.nameid);
    const companyId = toPositiveInt(claims.cid);
    const expiresAt = toPositiveInt(claims.exp);
    if (userId === null || companyId === null || expiresAt === null) {
      return null;
    }

    return {
      userId,
      companyId,
      username: typeof claims.unique_name === "string" ? claims.unique_name : "",
      permissions: typeof claims.permissions === "string" ? parsePermissionCodes(claims.permissions) : [],
      expiresAt,
    };
  } catch {
    return null;
  }
}

export function isExpired(identity: TokenIdentity, now = Date.now()): boolean {
  return identity.expiresAt * 1000 <= now;
}
