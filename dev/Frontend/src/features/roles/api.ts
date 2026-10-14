import "server-only";

import type * as z from "zod";
import type { Role, RolePermission } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { rolePermissionsSchema, roleSchema } from "./schema";

export function listRoles(filters: { description?: string; page: number; pageSize: number }) {
  return apiGet<Paged<Role>>("/roles", filters);
}

export function getRole(id: number) {
  return apiGetOptional<Role>(`/roles/${id}`);
}

export function createRole(input: z.output<typeof roleSchema>) {
  return apiCreate("/roles", input);
}

export function updateRole(id: number, input: z.output<typeof roleSchema>) {
  return apiSend("PUT", `/roles/${id}`, input);
}

export function deleteRole(id: number) {
  return apiSend("DELETE", `/roles/${id}`);
}

/** Every resource with its assignment for the role (the API returns all of them). */
export async function getRolePermissions(id: number): Promise<RolePermission[]> {
  return (await apiGet<Paged<RolePermission>>(`/roles/${id}/permissions`, { page: 1, pageSize: 100 })).items;
}

export function updateRolePermissions(id: number, permissions: z.output<typeof rolePermissionsSchema>) {
  return apiSend("PUT", `/roles/${id}/permissions`, permissions);
}
