import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { requiredText } from "@/lib/validation";

/** POST /grades, PUT /grades/{id} */
export const gradeSchema = z.object({
  description: requiredText(LIMITS.grade.description),
});

export type GradeFormValues = z.input<typeof gradeSchema>;
