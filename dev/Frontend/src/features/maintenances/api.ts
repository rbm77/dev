import "server-only";

import type * as z from "zod";
import type { CompletedMaintenance, Maintenance, PendingMaintenance } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { completeSchema } from "@/lib/schemas";
import type { completedMaintenanceSchema, maintenanceSchema } from "./schema";

export type MaintenanceFilters = { vehicleId?: number; type?: number; page: number; pageSize: number };

export type MaintenanceUpdate = z.output<typeof maintenanceSchema> | z.output<typeof completedMaintenanceSchema>;

export function listPendingMaintenances(filters: MaintenanceFilters) {
  return apiGet<Paged<PendingMaintenance>>("/maintenances/pending", filters);
}

export function listCompletedMaintenances(filters: MaintenanceFilters) {
  return apiGet<Paged<CompletedMaintenance>>("/maintenances/completed", filters);
}

export function getMaintenance(id: number) {
  return apiGetOptional<Maintenance>(`/maintenances/${id}`);
}

export function createMaintenance(input: z.output<typeof maintenanceSchema>) {
  return apiCreate("/maintenances", input);
}

export function updateMaintenance(id: number, input: MaintenanceUpdate) {
  return apiSend("PUT", `/maintenances/${id}`, input);
}

export function deleteMaintenance(id: number) {
  return apiSend("DELETE", `/maintenances/${id}`);
}

export function completeMaintenance(id: number, input: z.output<typeof completeSchema>) {
  return apiSend("PUT", `/maintenances/${id}/complete`, input);
}
