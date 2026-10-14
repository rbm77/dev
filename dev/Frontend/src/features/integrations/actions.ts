"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { emailAccountSchema, emailSenderSchema, whatsAppAccountSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const PATH = "/settings/integrations";

// Secrets received here are forwarded to the API only; they are never logged or returned.

export async function saveEmailAccount(id: unknown, input: unknown): Promise<ActionResult> {
  const accountId = id === null ? null : parseId(id);
  if (id !== null && !accountId) return INVALID_ID;
  const parsed = emailAccountSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EMAIL_ACCOUNT", "WRITE");
    if (accountId) {
      await api.updateEmailAccount(accountId, parsed.data);
    } else {
      await api.createEmailAccount(parsed.data);
    }
    revalidatePath(PATH);
  });
}

export async function deleteEmailAccount(id: unknown): Promise<ActionResult> {
  const accountId = parseId(id);
  if (!accountId) return INVALID_ID;

  return runAction(async () => {
    await authorize("EMAIL_ACCOUNT", "WRITE");
    await api.deleteEmailAccount(accountId);
    revalidatePath(PATH);
  });
}

export async function saveEmailSender(id: unknown, input: unknown): Promise<ActionResult> {
  const senderId = id === null ? null : parseId(id);
  if (id !== null && !senderId) return INVALID_ID;
  const parsed = emailSenderSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EMAIL_SENDER", "WRITE");
    if (senderId) {
      await api.updateEmailSender(senderId, parsed.data);
    } else {
      await api.createEmailSender(parsed.data);
    }
    revalidatePath(PATH);
  });
}

export async function deleteEmailSender(id: unknown): Promise<ActionResult> {
  const senderId = parseId(id);
  if (!senderId) return INVALID_ID;

  return runAction(async () => {
    await authorize("EMAIL_SENDER", "WRITE");
    await api.deleteEmailSender(senderId);
    revalidatePath(PATH);
  });
}

export async function saveWhatsAppAccount(id: unknown, input: unknown): Promise<ActionResult> {
  const accountId = id === null ? null : parseId(id);
  if (id !== null && !accountId) return INVALID_ID;
  const parsed = whatsAppAccountSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("WHATSAPP_ACCOUNT", "WRITE");
    if (accountId) {
      await api.updateWhatsAppAccount(accountId, parsed.data);
    } else {
      await api.createWhatsAppAccount(parsed.data);
    }
    revalidatePath(PATH);
  });
}

export async function deleteWhatsAppAccount(id: unknown): Promise<ActionResult> {
  const accountId = parseId(id);
  if (!accountId) return INVALID_ID;

  return runAction(async () => {
    await authorize("WHATSAPP_ACCOUNT", "WRITE");
    await api.deleteWhatsAppAccount(accountId);
    revalidatePath(PATH);
  });
}
