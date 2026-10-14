"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { contactSchema, qrCodesSchema, studentSchema } from "./schema";
import type { QrCode } from "./types";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };
const BASE64 = /^[A-Za-z0-9+/]+={0,2}$/;
const MAX_QR_BASE64_LENGTH = 2_000_000;

export async function createStudent(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = studentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("STUDENT", "WRITE");
    const id = await api.createStudent(parsed.data);
    revalidatePath("/students");
    return id;
  });
}

export async function updateStudent(id: unknown, input: unknown): Promise<ActionResult> {
  const studentId = parseId(id);
  if (!studentId) return INVALID_ID;
  const parsed = studentSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("STUDENT", "WRITE");
    await api.updateStudent(studentId, parsed.data);
    revalidatePath("/students");
    revalidatePath(`/students/${studentId}`);
  });
}

export async function deleteStudent(id: unknown): Promise<ActionResult> {
  const studentId = parseId(id);
  if (!studentId) return INVALID_ID;

  return runAction(async () => {
    await authorize("STUDENT", "WRITE");
    await api.deleteStudent(studentId);
    revalidatePath("/students");
  });
}

export async function createContact(studentId: unknown, input: unknown): Promise<ActionResult> {
  const id = parseId(studentId);
  if (!id) return INVALID_ID;
  const parsed = contactSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("CONTACT", "WRITE");
    await api.createContact(id, parsed.data);
    revalidatePath(`/students/${id}`);
  });
}

export async function updateContact(studentId: unknown, contactId: unknown, input: unknown): Promise<ActionResult> {
  const id = parseId(studentId);
  const contact = parseId(contactId);
  if (!id || !contact) return INVALID_ID;
  const parsed = contactSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("CONTACT", "WRITE");
    await api.updateContact(id, contact, parsed.data);
    revalidatePath(`/students/${id}`);
  });
}

export async function deleteContact(studentId: unknown, contactId: unknown): Promise<ActionResult> {
  const id = parseId(studentId);
  const contact = parseId(contactId);
  if (!id || !contact) return INVALID_ID;

  return runAction(async () => {
    await authorize("CONTACT", "WRITE");
    await api.deleteContact(id, contact);
    revalidatePath(`/students/${id}`);
  });
}

/** Only well-formed base64 PNG data is passed to the client to be rendered as an image. */
export async function generateQrCodes(input: unknown): Promise<ActionResult<QrCode[]>> {
  const parsed = qrCodesSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("STUDENT", "READ");
    const codes = await api.generateQrCodes(parsed.data);
    return codes
      .filter((code) => code.qrCodeBase64.length <= MAX_QR_BASE64_LENGTH && BASE64.test(code.qrCodeBase64))
      .map((code) => ({ description: code.description, qrCodeBase64: code.qrCodeBase64 }));
  });
}
