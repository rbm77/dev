"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { vehicleSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createVehicle(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = vehicleSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("VEHICLE", "WRITE");
    const id = await api.createVehicle(parsed.data);
    revalidatePath("/vehicles");
    return id;
  });
}

export async function updateVehicle(id: unknown, input: unknown): Promise<ActionResult> {
  const vehicleId = parseId(id);
  if (!vehicleId) return INVALID_ID;
  const parsed = vehicleSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("VEHICLE", "WRITE");
    await api.updateVehicle(vehicleId, parsed.data);
    revalidatePath("/vehicles");
    revalidatePath(`/vehicles/${vehicleId}`);
  });
}

export async function deleteVehicle(id: unknown): Promise<ActionResult> {
  const vehicleId = parseId(id);
  if (!vehicleId) return INVALID_ID;

  return runAction(async () => {
    await authorize("VEHICLE", "WRITE");
    await api.deleteVehicle(vehicleId);
    revalidatePath("/vehicles");
  });
}
