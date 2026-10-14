import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { MESSAGES, requiredText } from "@/lib/validation";

// Credentials (Features/Users/Credentials.cs). user.password is varchar(10).
export const loginSchema = z.object({
  username: requiredText(LIMITS.user.username),
  // Passwords are compared verbatim by the backend, so they are not trimmed.
  password: z
    .string({ error: MESSAGES.required })
    .min(1, MESSAGES.required)
    .max(LIMITS.user.password, MESSAGES.maxChars(LIMITS.user.password)),
});

export type LoginInput = z.input<typeof loginSchema>;

export const resetPasswordSchema = z.object({
  username: requiredText(LIMITS.user.username),
});

export type ResetPasswordInput = z.input<typeof resetPasswordSchema>;
