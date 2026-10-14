import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { entityId, isoDate, money, requiredText } from "@/lib/validation";

/** POST /payments, PUT /payments/{id}. receipt_reference is a global primary key (varchar 50). */
export const paymentSchema = z.object({
  studentId: entityId(),
  date: isoDate(),
  amount: money(),
  receiptReference: requiredText(LIMITS.payment.receiptReference),
});

export type PaymentFormValues = z.input<typeof paymentSchema>;
