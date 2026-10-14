"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { toExpenseKind } from "./kinds";
import { EXPENSE_SCHEMAS } from "./schema";

const INVALID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createExpense(kind: unknown, input: unknown): Promise<ActionResult> {
  const expenseKind = typeof kind === "string" ? toExpenseKind(kind) : null;
  if (!expenseKind) return INVALID;
  const parsed = EXPENSE_SCHEMAS[expenseKind].safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EXPENSE", "WRITE");
    await api.createExpense(expenseKind, parsed.data);
    revalidatePath(`/expenses/${expenseKind}`);
  });
}

export async function updateExpense(kind: unknown, id: unknown, input: unknown): Promise<ActionResult> {
  const expenseKind = typeof kind === "string" ? toExpenseKind(kind) : null;
  const expenseId = parseId(id);
  if (!expenseKind || !expenseId) return INVALID;
  const parsed = EXPENSE_SCHEMAS[expenseKind].safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("EXPENSE", "WRITE");
    await api.updateExpense(expenseKind, expenseId, parsed.data);
    revalidatePath(`/expenses/${expenseKind}`);
    revalidatePath(`/expenses/${expenseKind}/${expenseId}`);
  });
}

export async function deleteGeneralExpense(id: unknown): Promise<ActionResult> {
  const expenseId = parseId(id);
  if (!expenseId) return INVALID;

  return runAction(async () => {
    await authorize("EXPENSE", "WRITE");
    await api.deleteGeneralExpense(expenseId);
    revalidatePath("/expenses/general");
  });
}
