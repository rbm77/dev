import "server-only";

import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import { EXPENSE_KIND_INFO, type ExpenseKind } from "./kinds";
import type { ExpenseInput } from "./schema";
import type { Expense, ExpenseListItem } from "./types";

export type ExpenseFilters = {
  date?: string;
  vehicleId?: number;
  driverId?: number;
  maintenanceId?: number;
  employeeId?: number;
  incidentId?: number;
  page: number;
  pageSize: number;
};

export function listExpenses(kind: ExpenseKind, filters: ExpenseFilters) {
  return apiGet<Paged<ExpenseListItem>>(EXPENSE_KIND_INFO[kind].endpoint, filters);
}

export function getExpense(kind: ExpenseKind, id: number) {
  return apiGetOptional<Expense>(`${EXPENSE_KIND_INFO[kind].endpoint}/${id}`);
}

export function createExpense(kind: ExpenseKind, input: ExpenseInput) {
  return apiCreate(EXPENSE_KIND_INFO[kind].endpoint, input);
}

export function updateExpense(kind: ExpenseKind, id: number, input: ExpenseInput) {
  return apiSend("PUT", `${EXPENSE_KIND_INFO[kind].endpoint}/${id}`, input);
}

/** Only general expenses expose DELETE. */
export function deleteGeneralExpense(id: number) {
  return apiSend("DELETE", `/expenses/${id}`);
}
