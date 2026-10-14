import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { deleteGeneralExpense } from "@/features/expenses/actions";
import { getExpense } from "@/features/expenses/api";
import { ExpenseForm } from "@/features/expenses/components/expense-form";
import { EXPENSE_KIND_INFO, toExpenseKind } from "@/features/expenses/kinds";
import { getExpenseOptions } from "@/features/expenses/options";
import { formatDate } from "@/lib/dates";
import { formatMoney } from "@/lib/format";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Gasto" };

export default async function ExpensePage({ params }: PageProps<"/expenses/[kind]/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "EXPENSE", "READ")) {
    return <NoAccess />;
  }
  const { kind: rawKind, id: rawId } = await params;
  const kind = toExpenseKind(rawKind);
  const id = parseId(rawId);
  if (!kind || !id) {
    notFound();
  }
  const [expense, options] = await Promise.all([getExpense(kind, id), getExpenseOptions(kind)]);
  if (!expense) {
    notFound();
  }
  const canWrite = can(session.permissions, "EXPENSE", "WRITE");
  const info = EXPENSE_KIND_INFO[kind];

  return (
    <>
      <PageHeader
        title={`${formatMoney(expense.amount)} · ${formatDate(expense.date)}`}
        description={info.noun.charAt(0).toUpperCase() + info.noun.slice(1)}
        backHref={`/expenses/${kind}`}
        backLabel="Gastos"
        actions={
          canWrite &&
          info.deletable && (
            <DeleteButton action={deleteGeneralExpense.bind(null, id)} entityLabel="este gasto" redirectTo={`/expenses/${kind}`} />
          )
        }
      />
      <ExpenseForm kind={kind} expense={expense} options={options} readOnly={!canWrite} />
    </>
  );
}
