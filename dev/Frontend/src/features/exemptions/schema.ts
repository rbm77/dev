import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { entityId, isoDate, percentage, requiredText } from "@/lib/validation";

/** POST /specific-exemptions, PUT /specific-exemptions/{id}: discount on one payment period. */
export const specificExemptionSchema = z.object({
  studentId: entityId(),
  paymentPeriodId: entityId(),
  description: requiredText(LIMITS.specificExemption.description),
  percentage: percentage(),
});

export type SpecificExemptionFormValues = z.input<typeof specificExemptionSchema>;

/** POST /periodic-exemptions, PUT /periodic-exemptions/{id}: discount for every period in a date range. */
export const periodicExemptionSchema = z
  .object({
    studentId: entityId(),
    description: requiredText(LIMITS.periodicExemption.description),
    startDate: isoDate(),
    endDate: isoDate(),
    percentage: percentage(),
  })
  .refine((data) => data.endDate >= data.startDate, {
    path: ["endDate"],
    message: "La fecha final no puede ser anterior a la inicial.",
  });

export type PeriodicExemptionFormValues = z.input<typeof periodicExemptionSchema>;
