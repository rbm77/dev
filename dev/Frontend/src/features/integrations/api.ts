import "server-only";

import type * as z from "zod";
import { apiGet, apiSend, type Paged } from "@/lib/api";
import type { emailAccountSchema, emailSenderSchema, whatsAppAccountSchema } from "./schema";
import type { EmailAccount, EmailSender, WhatsAppAccount } from "./types";

type ListFilters = { isActive?: boolean; page: number; pageSize: number };

// E-mail accounts (IMAP mailboxes read by the e-mail ingestion service)
export function listEmailAccounts(filters: ListFilters) {
  return apiGet<Paged<EmailAccount>>("/email-accounts", filters);
}

export function createEmailAccount(input: z.output<typeof emailAccountSchema>) {
  return apiSend("POST", "/email-accounts", input);
}

export function updateEmailAccount(id: number, input: z.output<typeof emailAccountSchema>) {
  return apiSend("PUT", `/email-accounts/${id}`, input);
}

export function deleteEmailAccount(id: number) {
  return apiSend("DELETE", `/email-accounts/${id}`);
}

// E-mail senders (bank addresses whose notifications are trusted)
export function listEmailSenders(filters: ListFilters) {
  return apiGet<Paged<EmailSender>>("/email-senders", filters);
}

export function createEmailSender(input: z.output<typeof emailSenderSchema>) {
  return apiSend("POST", "/email-senders", input);
}

export function updateEmailSender(id: number, input: z.output<typeof emailSenderSchema>) {
  return apiSend("PUT", `/email-senders/${id}`, input);
}

export function deleteEmailSender(id: number) {
  return apiSend("DELETE", `/email-senders/${id}`);
}

// WhatsApp Business accounts used by the receipt bot
export function listWhatsAppAccounts(filters: ListFilters) {
  return apiGet<Paged<WhatsAppAccount>>("/whatsapp-accounts", filters);
}

export function createWhatsAppAccount(input: z.output<typeof whatsAppAccountSchema>) {
  return apiSend("POST", "/whatsapp-accounts", input);
}

export function updateWhatsAppAccount(id: number, input: z.output<typeof whatsAppAccountSchema>) {
  return apiSend("PUT", `/whatsapp-accounts/${id}`, input);
}

export function deleteWhatsAppAccount(id: number) {
  return apiSend("DELETE", `/whatsapp-accounts/${id}`);
}
