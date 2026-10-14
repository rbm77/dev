import "server-only";

import type * as z from "zod";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { completeSchema } from "@/lib/schemas";
import type { completedCustomTransportSchema, customTransportSchema } from "./schema";
import type { CompletedCustomTransport, CustomTransport, PendingCustomTransport } from "./types";

export type CustomTransportFilters = { vehicleId?: number; driverId?: number; page: number; pageSize: number };

export type CustomTransportUpdate =
  | z.output<typeof customTransportSchema>
  | z.output<typeof completedCustomTransportSchema>;

export function listPendingCustomTransports(filters: CustomTransportFilters) {
  return apiGet<Paged<PendingCustomTransport>>("/custom-transports/pending", filters);
}

export function listCompletedCustomTransports(filters: CustomTransportFilters) {
  return apiGet<Paged<CompletedCustomTransport>>("/custom-transports/completed", filters);
}

export function getCustomTransport(id: number) {
  return apiGetOptional<CustomTransport>(`/custom-transports/${id}`);
}

export function createCustomTransport(input: z.output<typeof customTransportSchema>) {
  return apiCreate("/custom-transports", input);
}

export function updateCustomTransport(id: number, input: CustomTransportUpdate) {
  return apiSend("PUT", `/custom-transports/${id}`, input);
}

export function deleteCustomTransport(id: number) {
  return apiSend("DELETE", `/custom-transports/${id}`);
}

export function completeCustomTransport(id: number, input: z.output<typeof completeSchema>) {
  return apiSend("PUT", `/custom-transports/${id}/complete`, input);
}
