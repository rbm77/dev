import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { entityId, isoDate, money, optionalText } from "@/lib/validation";

/** POST /payments, PUT /payments/{id}. receipt_reference is optional; when present it is a global primary key (varchar 50). */
export const paymentSchema = z.object({
  studentId: entityId(),
  date: isoDate(),
  amount: money(),
  receiptReference: optionalText(LIMITS.payment.receiptReference).refine(
    (value) => value === null || /^[A-Za-z0-9]+$/.test(value),
    "Solo letras y números, sin espacios, guiones ni otros símbolos.",
  ),
});

export type PaymentFormValues = z.input<typeof paymentSchema>;
