import "server-only";

import { ApiError, apiPostJson, apiSend } from "@/lib/api";

type Token = { authenticationToken?: string };

/** POST /users/authenticate. Returns the JWT, or null for invalid credentials (401). */
export async function authenticate(credentials: { username: string; password: string }): Promise<string | null> {
  try {
    const { authenticationToken } = await apiPostJson<Token>("/users/authenticate", credentials, { auth: false });
    return authenticationToken ?? null;
  } catch (error) {
    if (error instanceof ApiError && error.status === 401) {
      return null;
    }
    throw error;
  }
}

/** POST /users/reset-password. The API answers 202 whether or not the user exists. */
export async function requestPasswordReset(username: string): Promise<void> {
  await apiSend("POST", "/users/reset-password", { username }, { auth: false });
}
