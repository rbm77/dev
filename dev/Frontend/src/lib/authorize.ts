import "server-only";

import { ApiError } from "@/lib/api";
import { ERROR_MESSAGES } from "@/lib/errors";
import type { TokenIdentity } from "@/lib/jwt";
import { can, type PermissionMode, type Resource } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

/**
 * Server Action guard: requires a session and the given permission. Throws an
 * ApiError(403) that runAction turns into a friendly message. The API checks
 * again on its side; this avoids pointless calls and keeps actions self-authorizing.
 */
export async function authorize(resource: Resource, mode: PermissionMode): Promise<TokenIdentity> {
  const session = await verifySession();
  if (!can(session.permissions, resource, mode)) {
    throw new ApiError(403, ERROR_MESSAGES.forbidden);
  }
  return session;
}
