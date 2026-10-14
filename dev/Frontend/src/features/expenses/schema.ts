import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { entityId, integer, isoDate, money, optionalText } from "@/lib/validation";
import type { ExpenseKind } from "./kinds";

const base = {
  date: isoDate(),
  amount: money(),
  description: optionalText(LIMITS.expense.description),
};

/** Body of POST/PUT for each expense endpoint. Unknown keys are stripped by zod. */
export const EXPENSE_SCHEMAS = {
  general: z.object(base),
  fuel: z.object({ ...base, vehicleId: entityId(), driverId: entityId(), liters: integer(1) }),
  maintenance: z.object({ ...base, maintenanceId: entityId() }),
  salary: z.object({ ...base, employeeId: entityId() }),
  incident: z.object({ ...base, incidentId: entityId() }),
} satisfies Record<ExpenseKind, z.ZodType>;

export type ExpenseInput = z.output<(typeof EXPENSE_SCHEMAS)[ExpenseKind]>;

/** One form covers every kind; only the fields of the current kind are shown and sent. */
export type ExpenseFormValues = {
  date: string;
  amount: string;
  description: string;
  vehicleId: string;
  driverId: string;
  liters: string;
  maintenanceId: string;
  employeeId: string;
  incidentId: string;
};
