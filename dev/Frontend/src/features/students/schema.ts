import * as z from "zod";
import { LIMITS, NUMBER_LIMITS } from "@/lib/db-limits";
import { entityId, MESSAGES, optionalPhone, optionalText, requiredText } from "@/lib/validation";

/** POST /students, PUT /students/{id} */
export const studentSchema = z.object({
  name: requiredText(LIMITS.student.name),
  lastName: requiredText(LIMITS.student.lastName),
  identityDocument: optionalText(LIMITS.student.identityDocument),
  address: optionalText(LIMITS.student.address),
  routeId: entityId(),
  gradeId: entityId(),
  isActive: z.boolean(),
});

export type StudentFormValues = z.input<typeof studentSchema>;

/** POST /students/{id}/contacts, PUT /students/{id}/contacts/{contactId}. A contact needs a phone. */
export const contactSchema = z.object({
  phoneNumber: optionalPhone(LIMITS.contact.phoneNumber).refine((value) => value !== null, MESSAGES.required),
  description: optionalText(LIMITS.contact.description),
  isActive: z.boolean(),
});

export type ContactFormValues = z.input<typeof contactSchema>;

/** POST /students/qr-codes — the service generates at most 20 codes, size 1–50. */
export const qrCodesSchema = z.object({
  studentIds: z
    .array(z.number().int().positive())
    .min(1, "Selecciona al menos un estudiante.")
    .max(NUMBER_LIMITS.qrCodes.maxItems, `Máximo ${NUMBER_LIMITS.qrCodes.maxItems} estudiantes.`),
  size: z.number().int().min(1).max(NUMBER_LIMITS.qrCodes.maxSize),
});
