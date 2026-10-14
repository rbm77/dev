import "server-only";

import type * as z from "zod";
import type { Route } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiSend, type Paged } from "@/lib/api";
import type { routeSchema } from "./schema";

export function listRoutes(filters: { name?: string; isActive?: boolean; page: number; pageSize: number }) {
  return apiGet<Paged<Route>>("/routes", filters);
}

export function createRoute(input: z.output<typeof routeSchema>) {
  return apiCreate("/routes", input);
}

export function updateRoute(id: number, input: z.output<typeof routeSchema>) {
  return apiSend("PUT", `/routes/${id}`, input);
}

export function deleteRoute(id: number) {
  return apiSend("DELETE", `/routes/${id}`);
}
