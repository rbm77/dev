import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { optionalEmail, optionalPhone, requiredText } from "@/lib/validation";

/** Editable fields of PUT /companies/{id}. isActive is preserved server-side. */
export const companySchema = z.object({
  name: requiredText(LIMITS.company.name),
  phoneNumber: optionalPhone(LIMITS.company.phoneNumber),
  email: optionalEmail(LIMITS.company.email),
  autoApprovalEnabled: z.boolean(),
});

export type CompanyFormValues = z.input<typeof companySchema>;
