"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { completeSchema } from "@/lib/schemas";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { completedCustomTransportSchema, customTransportSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const PATH = "/custom-transports";

export async function createCustomTransport(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = customTransportSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("CUSTOM_TRANSPORT", "WRITE");
    const id = await api.createCustomTransport(parsed.data);
    revalidatePath(PATH);
    return id;
  });
}

export async function updateCustomTransport(id: unknown, input: unknown): Promise<ActionResult> {
  const transportId = parseId(id);
  if (!transportId) return INVALID_ID;
  const parsed = customTransportSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return saveChanges(transportId, parsed.data);
}

/** Edits a completed transport, which no longer has a scheduled date. */
export async function updateCompletedCustomTransport(id: unknown, input: unknown): Promise<ActionResult> {
  const transportId = parseId(id);
  if (!transportId) return INVALID_ID;
  const parsed = completedCustomTransportSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return saveChanges(transportId, parsed.data);
}

function saveChanges(id: number, input: api.CustomTransportUpdate): Promise<ActionResult> {
  return runAction(async () => {
    await authorize("CUSTOM_TRANSPORT", "WRITE");
    await api.updateCustomTransport(id, input);
    revalidatePath(PATH);
    revalidatePath(`${PATH}/${id}`);
  });
}

export async function deleteCustomTransport(id: unknown): Promise<ActionResult> {
  const transportId = parseId(id);
  if (!transportId) return INVALID_ID;

  return runAction(async () => {
    await authorize("CUSTOM_TRANSPORT", "WRITE");
    await api.deleteCustomTransport(transportId);
    revalidatePath(PATH);
  });
}

export async function completeCustomTransport(id: unknown, input: unknown): Promise<ActionResult> {
  const transportId = parseId(id);
  if (!transportId) return INVALID_ID;
  const parsed = completeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("CUSTOM_TRANSPORT", "WRITE");
    await api.completeCustomTransport(transportId, parsed.data);
    revalidatePath(PATH);
    revalidatePath(`${PATH}/${transportId}`);
  });
}
