"use client";

import { useRouter } from "next/navigation";
import type * as z from "zod";
import { DateField, IntegerField, MoneyField, SelectField, TextareaField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { createExpense, updateExpense } from "../actions";
import { EXPENSE_KIND_INFO, type ExpenseKind } from "../kinds";
import { EXPENSE_SCHEMAS, type ExpenseFormValues } from "../schema";
import type { Expense } from "../types";

export type ExpenseOptions = {
  vehicles?: Option[];
  drivers?: Option[];
  maintenances?: Option[];
  employees?: Option[];
  incidents?: Option[];
};

type ExpenseFormProps = {
  kind: ExpenseKind;
  expense?: Expense;
  options: ExpenseOptions;
  /** Reference preselected from another page (e.g. ?maintenanceId=5). */
  presetReference?: Partial<Pick<ExpenseFormValues, "maintenanceId" | "employeeId" | "incidentId">>;
  readOnly?: boolean;
};

const text = (value: number | undefined) => (value ? String(value) : "");

export function ExpenseForm({ kind, expense, options, presetReference, readOnly = false }: ExpenseFormProps) {
  const router = useRouter();
  const listPath = `/expenses/${kind}`;
  const { control, pending, onSubmit } = useActionForm<ExpenseFormValues>({
    // Each kind validates only its own fields; the shared form type is a superset.
    schema: EXPENSE_SCHEMAS[kind] as unknown as z.ZodType<unknown, ExpenseFormValues>,
    defaultValues: {
      date: expense?.date ?? "",
      amount: expense ? String(expense.amount) : "",
      description: expense?.description ?? "",
      vehicleId: text(expense?.vehicleId),
      driverId: text(expense?.driverId),
      liters: text(expense?.liters),
      maintenanceId: text(expense?.maintenanceId) || (presetReference?.maintenanceId ?? ""),
      employeeId: text(expense?.employeeId) || (presetReference?.employeeId ?? ""),
      incidentId: text(expense?.incidentId) || (presetReference?.incidentId ?? ""),
    },
    submit: (values) => (expense ? updateExpense(kind, expense.id, values) : createExpense(kind, values)),
    successMessage: expense ? "Gasto actualizado." : "Gasto registrado.",
    onSuccess: () => {
      if (!expense) router.push(listPath);
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title={`Datos del ${EXPENSE_KIND_INFO[kind].noun}`}>
          <DateField control={control} name="date" label="Fecha" required />
          <MoneyField control={control} name="amount" label="Monto (₡)" required />

          {kind === "fuel" && (
            <>
              <SelectField control={control} name="vehicleId" label="Vehículo" required options={options.vehicles ?? []} />
              <SelectField control={control} name="driverId" label="Conductor" required options={options.drivers ?? []} />
              <IntegerField control={control} name="liters" label="Litros" required />
            </>
          )}
          {kind === "maintenance" && (
            <SelectField control={control} name="maintenanceId" label="Mantenimiento" required options={options.maintenances ?? []} />
          )}
          {kind === "salary" && (
            <SelectField control={control} name="employeeId" label="Empleado" required options={options.employees ?? []} />
          )}
          {kind === "incident" && (
            <SelectField control={control} name="incidentId" label="Incidente" required options={options.incidents ?? []} />
          )}

          <TextareaField
            control={control}
            name="description"
            label="Descripción"
            maxLength={LIMITS.expense.description}
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={expense ? "Guardar cambios" : "Registrar gasto"} pending={pending} cancelHref={listPath} />
      )}
    </form>
  );
}
