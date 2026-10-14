"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { ApiError } from "@/lib/api";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { paymentSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const DUPLICATE_REFERENCE =
  "No se pudo guardar. Verifica que la referencia de recibo no esté registrada en otro pago.";

/** receipt_reference is unique: the API answers a duplicate with a generic 500. */
function explainDuplicate(error: unknown): never {
  if (error instanceof ApiError && error.status === 500) {
    throw new ApiError(500, DUPLICATE_REFERENCE);
  }
  throw error;
}

export async function createPayment(input: unknown): Promise<ActionResult> {
  const parsed = paymentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PAYMENT", "WRITE");
    await api.createPayment(parsed.data).catch(explainDuplicate);
    revalidatePath("/payments");
  });
}

export async function updatePayment(id: unknown, input: unknown): Promise<ActionResult> {
  const paymentId = parseId(id);
  if (!paymentId) return INVALID_ID;
  const parsed = paymentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("PAYMENT", "WRITE");
    await api.updatePayment(paymentId, parsed.data).catch(explainDuplicate);
    revalidatePath("/payments");
    revalidatePath(`/payments/${paymentId}`);
  });
}

export async function deletePayment(id: unknown): Promise<ActionResult> {
  const paymentId = parseId(id);
  if (!paymentId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PAYMENT", "WRITE");
    await api.deletePayment(paymentId);
    revalidatePath("/payments");
  });
}

export async function validatePaymentRequest(id: unknown): Promise<ActionResult> {
  const requestId = parseId(id);
  if (!requestId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PAYMENT_APPROVAL", "WRITE");
    await api.validatePaymentRequest(requestId);
    revalidatePath("/payment-requests");
  });
}

export async function approvePaymentRequest(id: unknown): Promise<ActionResult<number | null>> {
  const requestId = parseId(id);
  if (!requestId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PAYMENT_APPROVAL", "WRITE");
    try {
      const paymentId = await api.approvePaymentRequest(requestId);
      revalidatePath("/payment-requests");
      revalidatePath("/payments");
      return paymentId;
    } catch (error) {
      // 404 also means the request is not validated yet.
      if (error instanceof ApiError && error.status === 404) {
        throw new ApiError(404, "La solicitud debe estar validada para aprobarla, o ya no existe.");
      }
      throw error;
    }
  });
}

export async function rejectPaymentRequest(id: unknown): Promise<ActionResult> {
  const requestId = parseId(id);
  if (!requestId) return INVALID_ID;

  return runAction(async () => {
    await authorize("PAYMENT_APPROVAL", "WRITE");
    await api.rejectPaymentRequest(requestId);
    revalidatePath("/payment-requests");
  });
}
