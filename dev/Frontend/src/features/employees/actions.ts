"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { employeeSchema, salarySchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createEmployee(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = employeeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EMPLOYEE", "WRITE");
    const id = await api.createEmployee(parsed.data);
    revalidatePath("/employees");
    return id;
  });
}

export async function updateEmployee(id: unknown, input: unknown): Promise<ActionResult> {
  const employeeId = parseId(id);
  if (!employeeId) return INVALID_ID;
  const parsed = employeeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EMPLOYEE", "WRITE");
    await api.updateEmployee(employeeId, parsed.data);
    revalidatePath("/employees");
    revalidatePath(`/employees/${employeeId}`);
  });
}

export async function deleteEmployee(id: unknown): Promise<ActionResult> {
  const employeeId = parseId(id);
  if (!employeeId) return INVALID_ID;

  return runAction(async () => {
    await authorize("EMPLOYEE", "WRITE");
    await api.deleteEmployee(employeeId);
    revalidatePath("/employees");
  });
}

export async function createSalary(employeeId: unknown, input: unknown): Promise<ActionResult> {
  const id = parseId(employeeId);
  if (!id) return INVALID_ID;
  const parsed = salarySchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("SALARY", "WRITE");
    await api.createSalary(id, parsed.data);
    revalidatePath(`/employees/${id}`);
  });
}

export async function deleteSalary(employeeId: unknown, salaryId: unknown): Promise<ActionResult> {
  const id = parseId(employeeId);
  const salary = parseId(salaryId);
  if (!id || !salary) return INVALID_ID;

  return runAction(async () => {
    await authorize("SALARY", "WRITE");
    await api.deleteSalary(id, salary);
    revalidatePath(`/employees/${id}`);
  });
}
