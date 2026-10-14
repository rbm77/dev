"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { paymentPeriodRequestSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const PATH = "/payment-periods";

export async function createPaymentPeriodRequest(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = paymentPeriodRequestSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PAYMENT_PERIOD", "WRITE");
    const id = await api.createPaymentPeriodRequest(parsed.data);
    revalidatePath(PATH);
    return id;
  });
}

export async function updatePaymentPeriodRequest(id: unknown, input: unknown): Promise<ActionResult> {
  const requestId = parseId(id);
  if (!requestId) return INVALID_ID;
  const parsed = paymentPeriodRequestSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PAYMENT_PERIOD", "WRITE");
    await api.updatePaymentPeriodRequest(requestId, parsed.data);
    revalidatePath(PATH);
    revalidatePath(`${PATH}/${requestId}`);
  });
}

export async function deletePaymentPeriodRequest(id: unknown): Promise<ActionResult> {
  const requestId = parseId(id);
  if (!requestId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PAYMENT_PERIOD", "WRITE");
    await api.deletePaymentPeriodRequest(requestId);
    revalidatePath(PATH);
  });
}
