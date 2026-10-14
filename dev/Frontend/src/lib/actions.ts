import "server-only";

import { unstable_rethrow } from "next/navigation";
import * as z from "zod";
import type { ActionResult } from "@/lib/action-result";
import { ApiError } from "@/lib/api";
import { ERROR_MESSAGES } from "@/lib/errors";

/**
 * Runs the body of a Server Action and turns failures into an ActionResult.
 * Next.js control-flow errors (redirect, notFound) are rethrown untouched.
 */
export async function runAction<T>(body: () => Promise<T>): Promise<ActionResult<T>> {
  try {
    return { ok: true, data: await body() };
  } catch (error) {
    unstable_rethrow(error);
    if (error instanceof ApiError) {
      return { ok: false, error: error.message };
    }
    console.error("[action] unexpected error:", error);
    return { ok: false, error: ERROR_MESSAGES.server };
  }
}

/** Result for input that failed server-side validation. */
export function invalidInput(error: z.ZodError): ActionResult<never> {
  return {
    ok: false,
    error: ERROR_MESSAGES.badRequest,
    fieldErrors: z.flattenError(error).fieldErrors as Partial<Record<string, string[]>>,
  };
}
