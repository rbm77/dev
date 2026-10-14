import "server-only";

import type * as z from "zod";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { CriticalProcessUser, User, UserListItem } from "./types";
import type { ownUserSchema, userSchema } from "./schema";

export type UserFilters = {
  name?: string;
  lastName?: string;
  identityDocument?: string;
  roleId?: number;
  isActive?: boolean;
  page: number;
  pageSize: number;
};

export function listUsers(filters: UserFilters) {
  return apiGet<Paged<UserListItem>>("/users", filters);
}

export function getUser(id: number) {
  return apiGetOptional<User>(`/users/${id}`);
}

export function createUser(input: z.output<typeof userSchema>) {
  return apiCreate("/users", input);
}

export function updateUser(id: number, input: z.output<typeof userSchema>) {
  return apiSend("PUT", `/users/${id}`, input);
}

export function deleteUser(id: number) {
  return apiSend("DELETE", `/users/${id}`);
}

export function getOwnUser(id: number) {
  return apiGetOptional<User>(`/users/${id}/own`);
}

export function updateOwnUser(id: number, input: z.output<typeof ownUserSchema>) {
  return apiSend("PUT", `/users/${id}/own`, input);
}

export function changeOwnPassword(id: number, password: string) {
  return apiSend("PUT", `/users/${id}/password`, { password });
}

export async function listCriticalProcessUsers(): Promise<CriticalProcessUser[]> {
  return (await apiGet<Paged<CriticalProcessUser>>("/users/critical-process", { page: 1, pageSize: 500 })).items;
}

export function replaceCriticalProcessUsers(ids: number[]) {
  return apiSend(
    "PUT",
    "/users/critical-process",
    ids.map((id) => ({ id })),
  );
}
