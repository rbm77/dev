import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { ExpenseForm } from "@/features/expenses/components/expense-form";
import { EXPENSE_KIND_INFO, toExpenseKind } from "@/features/expenses/kinds";
import { getExpenseOptions } from "@/features/expenses/options";
import { can } from "@/lib/permissions";
import { getIdParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Registrar gasto" };

const asText = (id: number | undefined) => (id ? String(id) : undefined);

export default async function NewExpensePage({ params, searchParams }: PageProps<"/expenses/[kind]/new">) {
  const session = await verifySession();
  if (!can(session.permissions, "EXPENSE", "WRITE")) {
    return <NoAccess />;
  }
  const kind = toExpenseKind((await params).kind);
  if (!kind) {
    notFound();
  }
  const query = await searchParams;

  return (
    <>
      <PageHeader
        title={`Registrar ${EXPENSE_KIND_INFO[kind].noun}`}
        backHref={`/expenses/${kind}`}
        backLabel="Gastos"
      />
      <ExpenseForm
        kind={kind}
        options={await getExpenseOptions(kind)}
        presetReference={{
          maintenanceId: asText(getIdParam(query, "maintenanceId")),
          employeeId: asText(getIdParam(query, "employeeId")),
          incidentId: asText(getIdParam(query, "incidentId")),
        }}
      />
    </>
  );
}
