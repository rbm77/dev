import "server-only";

import type * as z from "zod";
import type { Vehicle, VehicleListItem } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { vehicleSchema } from "./schema";

export type VehicleFilters = {
  licensePlate?: string;
  make?: string;
  model?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
};

export function listVehicles(filters: VehicleFilters) {
  return apiGet<Paged<VehicleListItem>>("/vehicles", filters);
}

export function getVehicle(id: number) {
  return apiGetOptional<Vehicle>(`/vehicles/${id}`);
}

export function createVehicle(input: z.output<typeof vehicleSchema>) {
  return apiCreate("/vehicles", input);
}

export function updateVehicle(id: number, input: z.output<typeof vehicleSchema>) {
  return apiSend("PUT", `/vehicles/${id}`, input);
}

export function deleteVehicle(id: number) {
  return apiSend("DELETE", `/vehicles/${id}`);
}
