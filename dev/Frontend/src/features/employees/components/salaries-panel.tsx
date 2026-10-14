"use client";

import { useState } from "react";
import { Plus, Wallet } from "lucide-react";
import { MoneyField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { DeleteButton } from "@/components/shared/delete-button";
import { EmptyState } from "@/components/shared/empty-state";
import { DateText, Money } from "@/components/shared/formatted";
import { StatusBadge } from "@/components/shared/status-badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { formatMoney } from "@/lib/format";
import { createSalary, deleteSalary } from "../actions";
import { salarySchema, type SalaryFormValues } from "../schema";
import type { Salary } from "../types";

function AddSalaryDialog({ employeeId, open, onOpenChange }: { employeeId: number; open: boolean; onOpenChange: (open: boolean) => void }) {
  const { form, control, pending, onSubmit } = useActionForm<SalaryFormValues>({
    schema: salarySchema,
    defaultValues: { amount: "" },
    submit: (values) => createSalary(employeeId, values),
    successMessage: "Salario registrado.",
    onSuccess: () => {
      onOpenChange(false);
      form.reset();
    },
  });

  return (
    <FormDialog
      open={open}
      onOpenChange={onOpenChange}
      title="Nuevo salario"
      description="Rige desde hoy y reemplaza al salario vigente."
      onSubmit={onSubmit}
      pending={pending}
    >
      <MoneyField control={control} name="amount" label="Monto mensual (₡)" required />
    </FormDialog>
  );
}

type SalariesPanelProps = { employeeId: number; salaries: Salary[]; canWrite: boolean };

export function SalariesPanel({ employeeId, salaries, canWrite }: SalariesPanelProps) {
  const [open, setOpen] = useState(false);
  // Most recent first; the first one is the salary in force.
  const ordered = [...salaries].sort((a, b) => (b.startDate ?? "").localeCompare(a.startDate ?? "") || b.id - a.id);

  return (
    <div className="flex flex-col gap-4">
      {canWrite && (
        <div className="flex justify-end">
          <Button onClick={() => setOpen(true)}>
            <Plus aria-hidden />
            Nuevo salario
          </Button>
          <AddSalaryDialog employeeId={employeeId} open={open} onOpenChange={setOpen} />
        </div>
      )}

      {ordered.length === 0 ? (
        <EmptyState icon={Wallet} title="Sin salarios registrados" />
      ) : (
        <Card className="gap-0 py-0">
          <ul className="divide-y">
            {ordered.map((salary, index) => (
              <li key={salary.id} className="flex items-center justify-between gap-3 px-4 py-3">
                <div className="flex flex-col gap-0.5">
                  <Money value={salary.amount} className="font-medium" />
                  <span className="text-sm text-muted-foreground">
                    Desde <DateText value={salary.startDate} />
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  {index === 0 && <StatusBadge tone="success">Vigente</StatusBadge>}
                  {canWrite && (
                    <DeleteButton
                      action={() => deleteSalary(employeeId, salary.id)}
                      entityLabel={`el salario de ${formatMoney(salary.amount)}`}
                      compact
                    />
                  )}
                </div>
              </li>
            ))}
          </ul>
        </Card>
      )}
    </div>
  );
}
