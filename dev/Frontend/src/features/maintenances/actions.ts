"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { completeSchema } from "@/lib/schemas";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { completedMaintenanceSchema, maintenanceSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createMaintenance(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = maintenanceSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("MAINTENANCE", "WRITE");
    const id = await api.createMaintenance(parsed.data);
    revalidatePath("/maintenances");
    return id;
  });
}

export async function updateMaintenance(id: unknown, input: unknown): Promise<ActionResult> {
  const maintenanceId = parseId(id);
  if (!maintenanceId) return INVALID_ID;
  const parsed = maintenanceSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return saveChanges(maintenanceId, parsed.data);
}

/** Edits a completed maintenance, which no longer has a scheduled date. */
export async function updateCompletedMaintenance(id: unknown, input: unknown): Promise<ActionResult> {
  const maintenanceId = parseId(id);
  if (!maintenanceId) return INVALID_ID;
  const parsed = completedMaintenanceSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return saveChanges(maintenanceId, parsed.data);
}

function saveChanges(id: number, input: api.MaintenanceUpdate): Promise<ActionResult> {
  return runAction(async () => {
    await authorize("MAINTENANCE", "WRITE");
    await api.updateMaintenance(id, input);
    revalidatePath("/maintenances");
    revalidatePath(`/maintenances/${id}`);
  });
}

export async function deleteMaintenance(id: unknown): Promise<ActionResult> {
  const maintenanceId = parseId(id);
  if (!maintenanceId) return INVALID_ID;

  return runAction(async () => {
    await authorize("MAINTENANCE", "WRITE");
    await api.deleteMaintenance(maintenanceId);
    revalidatePath("/maintenances");
  });
}

export async function completeMaintenance(id: unknown, input: unknown): Promise<ActionResult> {
  const maintenanceId = parseId(id);
  if (!maintenanceId) return INVALID_ID;
  const parsed = completeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("MAINTENANCE", "WRITE");
    await api.completeMaintenance(maintenanceId, parsed.data);
    revalidatePath("/maintenances");
    revalidatePath(`/maintenances/${maintenanceId}`);
  });
}
