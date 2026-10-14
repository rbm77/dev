import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { isoDate, requiredText } from "@/lib/validation";

/** POST /vacation, PUT /vacation/{id} */
export const vacationSchema = z
  .object({
    description: requiredText(LIMITS.vacation.description),
    startDate: isoDate(),
    endDate: isoDate(),
  })
  .refine((data) => data.endDate >= data.startDate, {
    path: ["endDate"],
    message: "La fecha final no puede ser anterior a la inicial.",
  });

export type VacationFormValues = z.input<typeof vacationSchema>;
