import * as z from "zod";
import { NUMBER_LIMITS } from "@/lib/db-limits";
import { integer, isoDate, money } from "@/lib/validation";

/** POST /payment-period-requests, PUT /payment-period-requests/{id} */
export const paymentPeriodRequestSchema = z.object({
  amount: money(),
  startDate: isoDate(),
  daysToNextPayment: integer(NUMBER_LIMITS.daysToNextPayment.min, NUMBER_LIMITS.daysToNextPayment.max),
});

export type PaymentPeriodRequestFormValues = z.input<typeof paymentPeriodRequestSchema>;
