"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { vacationSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createVacation(input: unknown): Promise<ActionResult> {
  const parsed = vacationSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("VACATION", "WRITE");
    await api.createVacation(parsed.data);
    revalidatePath("/vacations");
  });
}

export async function updateVacation(id: unknown, input: unknown): Promise<ActionResult> {
  const vacationId = parseId(id);
  if (!vacationId) return INVALID_ID;
  const parsed = vacationSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("VACATION", "WRITE");
    await api.updateVacation(vacationId, parsed.data);
    revalidatePath("/vacations");
    revalidatePath(`/vacations/${vacationId}`);
  });
}

export async function deleteVacation(id: unknown): Promise<ActionResult> {
  const vacationId = parseId(id);
  if (!vacationId) return INVALID_ID;

  return runAction(async () => {
    await authorize("VACATION", "WRITE");
    await api.deleteVacation(vacationId);
    revalidatePath("/vacations");
  });
}
