import "server-only";

import type * as z from "zod";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { periodicExemptionSchema, specificExemptionSchema } from "./schema";
import type { PeriodicExemption, PeriodicExemptionListItem, SpecificExemption, SpecificExemptionListItem } from "./types";

// Specific exemptions
export function listSpecificExemptions(filters: { studentId?: number; paymentPeriodId?: number; page: number; pageSize: number }) {
  return apiGet<Paged<SpecificExemptionListItem>>("/specific-exemptions", filters);
}

export function getSpecificExemption(id: number) {
  return apiGetOptional<SpecificExemption>(`/specific-exemptions/${id}`);
}

export function createSpecificExemption(input: z.output<typeof specificExemptionSchema>) {
  return apiCreate("/specific-exemptions", input);
}

export function updateSpecificExemption(id: number, input: z.output<typeof specificExemptionSchema>) {
  return apiSend("PUT", `/specific-exemptions/${id}`, input);
}

export function deleteSpecificExemption(id: number) {
  return apiSend("DELETE", `/specific-exemptions/${id}`);
}

// Periodic exemptions
export function listPeriodicExemptions(filters: { studentId?: number; page: number; pageSize: number }) {
  return apiGet<Paged<PeriodicExemptionListItem>>("/periodic-exemptions", filters);
}

export function getPeriodicExemption(id: number) {
  return apiGetOptional<PeriodicExemption>(`/periodic-exemptions/${id}`);
}

export function createPeriodicExemption(input: z.output<typeof periodicExemptionSchema>) {
  return apiCreate("/periodic-exemptions", input);
}

export function updatePeriodicExemption(id: number, input: z.output<typeof periodicExemptionSchema>) {
  return apiSend("PUT", `/periodic-exemptions/${id}`, input);
}

export function deletePeriodicExemption(id: number) {
  return apiSend("DELETE", `/periodic-exemptions/${id}`);
}
