import "server-only";

import type * as z from "zod";
import type { Student, StudentListItem } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiPostJson, apiSend, type Paged } from "@/lib/api";
import type { contactSchema, qrCodesSchema, studentSchema } from "./schema";
import type { Contact, QrCode } from "./types";

export type StudentFilters = {
  name?: string;
  lastName?: string;
  identityDocument?: string;
  routeId?: number;
  gradeId?: number;
  isActive?: boolean;
  page: number;
  pageSize: number;
};

export function listStudents(filters: StudentFilters) {
  return apiGet<Paged<StudentListItem>>("/students", filters);
}

export function getStudent(id: number) {
  return apiGetOptional<Student>(`/students/${id}`);
}

export function createStudent(input: z.output<typeof studentSchema>) {
  return apiCreate("/students", input);
}

export function updateStudent(id: number, input: z.output<typeof studentSchema>) {
  return apiSend("PUT", `/students/${id}`, input);
}

export function deleteStudent(id: number) {
  return apiSend("DELETE", `/students/${id}`);
}

export async function listContacts(studentId: number): Promise<Contact[]> {
  return (await apiGet<Paged<Contact>>(`/students/${studentId}/contacts`, { page: 1, pageSize: 100 })).items;
}

export function createContact(studentId: number, input: z.output<typeof contactSchema>) {
  return apiCreate(`/students/${studentId}/contacts`, input);
}

export function updateContact(studentId: number, id: number, input: z.output<typeof contactSchema>) {
  return apiSend("PUT", `/students/${studentId}/contacts/${id}`, input);
}

export function deleteContact(studentId: number, id: number) {
  return apiSend("DELETE", `/students/${studentId}/contacts/${id}`);
}

export function generateQrCodes(input: z.output<typeof qrCodesSchema>) {
  return apiPostJson<QrCode[]>("/students/qr-codes", input);
}
