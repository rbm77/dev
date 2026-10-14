import "server-only";

import type * as z from "zod";
import type { Grade } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiSend, type Paged } from "@/lib/api";
import type { gradeSchema } from "./schema";

export function listGrades(filters: { description?: string; page: number; pageSize: number }) {
  return apiGet<Paged<Grade>>("/grades", filters);
}

export function createGrade(input: z.output<typeof gradeSchema>) {
  return apiCreate("/grades", input);
}

export function updateGrade(id: number, input: z.output<typeof gradeSchema>) {
  return apiSend("PUT", `/grades/${id}`, input);
}

export function deleteGrade(id: number) {
  return apiSend("DELETE", `/grades/${id}`);
}
