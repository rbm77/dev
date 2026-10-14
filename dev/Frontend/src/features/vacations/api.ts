import "server-only";

import type * as z from "zod";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { vacationSchema } from "./schema";
import type { Vacation, VacationListItem } from "./types";

// Note: the controller is VacationController, so the route is singular.
export function listVacations(page: number, pageSize: number) {
  return apiGet<Paged<VacationListItem>>("/vacation", { page, pageSize });
}

export function getVacation(id: number) {
  return apiGetOptional<Vacation>(`/vacation/${id}`);
}

export function createVacation(input: z.output<typeof vacationSchema>) {
  return apiCreate("/vacation", input);
}

export function updateVacation(id: number, input: z.output<typeof vacationSchema>) {
  return apiSend("PUT", `/vacation/${id}`, input);
}

export function deleteVacation(id: number) {
  return apiSend("DELETE", `/vacation/${id}`);
}
