"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { periodicExemptionSchema, specificExemptionSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const SPECIFIC = "/exemptions/specific";
const PERIODIC = "/exemptions/periodic";

export async function createSpecificExemption(input: unknown): Promise<ActionResult> {
  const parsed = specificExemptionSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("SPECIFIC_EXEMPTION", "WRITE");
    await api.createSpecificExemption(parsed.data);
    revalidatePath(SPECIFIC);
  });
}

export async function updateSpecificExemption(id: unknown, input: unknown): Promise<ActionResult> {
  const exemptionId = parseId(id);
  if (!exemptionId) return INVALID_ID;
  const parsed = specificExemptionSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("SPECIFIC_EXEMPTION", "WRITE");
    await api.updateSpecificExemption(exemptionId, parsed.data);
    revalidatePath(SPECIFIC);
    revalidatePath(`${SPECIFIC}/${exemptionId}`);
  });
}

export async function deleteSpecificExemption(id: unknown): Promise<ActionResult> {
  const exemptionId = parseId(id);
  if (!exemptionId) return INVALID_ID;

  return runAction(async () => {
    await authorize("SPECIFIC_EXEMPTION", "WRITE");
    await api.deleteSpecificExemption(exemptionId);
    revalidatePath(SPECIFIC);
  });
}

export async function createPeriodicExemption(input: unknown): Promise<ActionResult> {
  const parsed = periodicExemptionSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PERIODIC_EXEMPTION", "WRITE");
    await api.createPeriodicExemption(parsed.data);
    revalidatePath(PERIODIC);
  });
}

export async function updatePeriodicExemption(id: unknown, input: unknown): Promise<ActionResult> {
  const exemptionId = parseId(id);
  if (!exemptionId) return INVALID_ID;
  const parsed = periodicExemptionSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PERIODIC_EXEMPTION", "WRITE");
    await api.updatePeriodicExemption(exemptionId, parsed.data);
    revalidatePath(PERIODIC);
    revalidatePath(`${PERIODIC}/${exemptionId}`);
  });
}

export async function deletePeriodicExemption(id: unknown): Promise<ActionResult> {
  const exemptionId = parseId(id);
  if (!exemptionId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PERIODIC_EXEMPTION", "WRITE");
    await api.deletePeriodicExemption(exemptionId);
    revalidatePath(PERIODIC);
  });
}
