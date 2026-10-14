"use server";

import { redirect } from "next/navigation";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { createSession, deleteSession } from "@/lib/session";
import { authenticate, requestPasswordReset } from "./api";
import { loginSchema, resetPasswordSchema } from "./schema";

const INVALID_CREDENTIALS = "Usuario o contraseña incorrectos.";

// Only same-site relative paths are accepted to avoid open redirects.
function safeRedirectPath(next: unknown): string {
  if (typeof next !== "string" || !next.startsWith("/") || next.startsWith("//") || next.startsWith("/\\")) {
    return "/";
  }
  if (next.startsWith("/login") || next.startsWith("/forgot-password") || next.startsWith("/session-expired")) {
    return "/";
  }
  return next;
}

export async function login(input: unknown, next?: unknown): Promise<ActionResult> {
  const parsed = loginSchema.safeParse(input);
  if (!parsed.success) {
    return invalidInput(parsed.error);
  }

  const result = await runAction(() => authenticate(parsed.data));
  if (!result.ok) {
    return result;
  }
  if (!result.data || !(await createSession(result.data))) {
    return { ok: false, error: INVALID_CREDENTIALS };
  }

  redirect(safeRedirectPath(next));
}

export async function logout(): Promise<void> {
  await deleteSession();
  redirect("/login");
}

export async function resetPassword(input: unknown): Promise<ActionResult> {
  const parsed = resetPasswordSchema.safeParse(input);
  if (!parsed.success) {
    return invalidInput(parsed.error);
  }

  return runAction(() => requestPasswordReset(parsed.data.username));
}
