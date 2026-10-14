import "server-only";

import type * as z from "zod";
import type { Incident, IncidentListItem } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { incidentSchema } from "./schema";

export type IncidentFilters = { vehicleId?: number; driverId?: number; type?: number; page: number; pageSize: number };

export function listIncidents(filters: IncidentFilters) {
  return apiGet<Paged<IncidentListItem>>("/incidents", filters);
}

export function getIncident(id: number) {
  return apiGetOptional<Incident>(`/incidents/${id}`);
}

export function createIncident(input: z.output<typeof incidentSchema>) {
  return apiCreate("/incidents", input);
}

export function updateIncident(id: number, input: z.output<typeof incidentSchema>) {
  return apiSend("PUT", `/incidents/${id}`, input);
}

export function deleteIncident(id: number) {
  return apiSend("DELETE", `/incidents/${id}`);
}
