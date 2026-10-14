"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { incidentSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createIncident(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = incidentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("INCIDENT", "WRITE");
    const id = await api.createIncident(parsed.data);
    revalidatePath("/incidents");
    return id;
  });
}

export async function updateIncident(id: unknown, input: unknown): Promise<ActionResult> {
  const incidentId = parseId(id);
  if (!incidentId) return INVALID_ID;
  const parsed = incidentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("INCIDENT", "WRITE");
    await api.updateIncident(incidentId, parsed.data);
    revalidatePath("/incidents");
    revalidatePath(`/incidents/${incidentId}`);
  });
}

export async function deleteIncident(id: unknown): Promise<ActionResult> {
  const incidentId = parseId(id);
  if (!incidentId) return INVALID_ID;

  return runAction(async () => {
    await authorize("INCIDENT", "WRITE");
    await api.deleteIncident(incidentId);
    revalidatePath("/incidents");
  });
}
