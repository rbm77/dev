import * as z from "zod";
import { LIMITS, NUMBER_LIMITS } from "@/lib/db-limits";
import { integer, MESSAGES, optionalText, requiredEmail, requiredText } from "@/lib/validation";

// Secrets are not trimmed (they are used verbatim) and are required on every
// save because the update procedures overwrite them.
function secret(max: number) {
  return z.string({ error: MESSAGES.required }).min(1, MESSAGES.required).max(max, MESSAGES.maxChars(max));
}

/** POST /email-accounts, PUT /email-accounts/{id} */
export const emailAccountSchema = z.object({
  emailAddress: requiredEmail(LIMITS.emailAccount.emailAddress),
  appPassword: secret(LIMITS.emailAccount.appPassword),
  imapHost: requiredText(LIMITS.emailAccount.imapHost),
  imapPort: integer(NUMBER_LIMITS.imapPort.min, NUMBER_LIMITS.imapPort.max),
  isActive: z.boolean(),
});

export type EmailAccountFormValues = z.input<typeof emailAccountSchema>;

/** POST /email-senders, PUT /email-senders/{id} */
export const emailSenderSchema = z.object({
  senderAddress: requiredEmail(LIMITS.emailSender.senderAddress),
  description: optionalText(LIMITS.emailSender.description),
  isActive: z.boolean(),
});

export type EmailSenderFormValues = z.input<typeof emailSenderSchema>;

/** POST /whatsapp-accounts, PUT /whatsapp-accounts/{id} */
export const whatsAppAccountSchema = z.object({
  phoneNumberId: requiredText(LIMITS.whatsappAccount.phoneNumberId).regex(/^\d+$/, "Solo se permiten números."),
  accessToken: secret(LIMITS.whatsappAccount.accessToken),
  isActive: z.boolean(),
});

export type WhatsAppAccountFormValues = z.input<typeof whatsAppAccountSchema>;
