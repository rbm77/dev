"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { gradeSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createGrade(input: unknown): Promise<ActionResult> {
  const parsed = gradeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("GRADE", "WRITE");
    await api.createGrade(parsed.data);
    revalidatePath("/grades");
  });
}

export async function updateGrade(id: unknown, input: unknown): Promise<ActionResult> {
  const gradeId = parseId(id);
  if (!gradeId) return INVALID_ID;
  const parsed = gradeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("GRADE", "WRITE");
    await api.updateGrade(gradeId, parsed.data);
    revalidatePath("/grades");
  });
}

export async function deleteGrade(id: unknown): Promise<ActionResult> {
  const gradeId = parseId(id);
  if (!gradeId) return INVALID_ID;

  return runAction(async () => {
    await authorize("GRADE", "WRITE");
    await api.deleteGrade(gradeId);
    revalidatePath("/grades");
  });
}
