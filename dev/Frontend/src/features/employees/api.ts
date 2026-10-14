import "server-only";

import type * as z from "zod";
import type { Employee, EmployeeListItem } from "@/features/catalogs/types";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { employeeSchema, salarySchema } from "./schema";
import type { Salary } from "./types";

export type EmployeeFilters = {
  name?: string;
  lastName?: string;
  identityDocument?: string;
  isActive?: boolean;
  page: number;
  pageSize: number;
};

export function listEmployees(filters: EmployeeFilters) {
  return apiGet<Paged<EmployeeListItem>>("/employees", filters);
}

export function getEmployee(id: number) {
  return apiGetOptional<Employee>(`/employees/${id}`);
}

export function createEmployee(input: z.output<typeof employeeSchema>) {
  return apiCreate("/employees", input);
}

export function updateEmployee(id: number, input: z.output<typeof employeeSchema>) {
  return apiSend("PUT", `/employees/${id}`, input);
}

export function deleteEmployee(id: number) {
  return apiSend("DELETE", `/employees/${id}`);
}

export async function listSalaries(employeeId: number): Promise<Salary[]> {
  return (await apiGet<Paged<Salary>>(`/employees/${employeeId}/salaries`, { page: 1, pageSize: 100 })).items;
}

export function createSalary(employeeId: number, input: z.output<typeof salarySchema>) {
  return apiSend("POST", `/employees/${employeeId}/salaries`, input);
}

export function deleteSalary(employeeId: number, id: number) {
  return apiSend("DELETE", `/employees/${employeeId}/salaries/${id}`);
}
