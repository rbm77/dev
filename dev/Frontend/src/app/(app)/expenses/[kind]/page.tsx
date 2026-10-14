import { Suspense } from "react";
import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { Receipt } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar, type FilterField } from "@/components/shared/filter-bar";
import { DateText, Money } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { TabLinks } from "@/components/shared/tab-links";
import { listExpenses } from "@/features/expenses/api";
import { EXPENSE_KIND_INFO, EXPENSE_KINDS, toExpenseKind, type ExpenseKind } from "@/features/expenses/kinds";
import { getExpenseOptions } from "@/features/expenses/options";
import { can } from "@/lib/permissions";
import { getDateParam, getIdParam, getPage, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Gastos" };

const REFERENCE_FILTERS = ["vehicleId", "driverId", "maintenanceId", "employeeId", "incidentId"] as const;

export default async function ExpensesPage({ params, searchParams }: PageProps<"/expenses/[kind]">) {
  const session = await verifySession();
  if (!can(session.permissions, "EXPENSE", "READ")) {
    return <NoAccess />;
  }
  const kind = toExpenseKind((await params).kind);
  if (!kind) {
    notFound();
  }
  const query = await searchParams;
  const options = await getExpenseOptions(kind);

  const fields: FilterField[] = [{ type: "date", name: "date", label: "Fecha" }];
  if (options.vehicles) fields.push({ type: "select", name: "vehicleId", label: "Vehículo", options: options.vehicles });
  if (options.drivers) fields.push({ type: "select", name: "driverId", label: "Conductor", options: options.drivers });
  if (options.maintenances) fields.push({ type: "select", name: "maintenanceId", label: "Mantenimiento", options: options.maintenances });
  if (options.employees) fields.push({ type: "select", name: "employeeId", label: "Empleado", options: options.employees });
  if (options.incidents) fields.push({ type: "select", name: "incidentId", label: "Incidente", options: options.incidents });

  return (
    <>
      <PageHeader
        title="Gastos"
        description="Egresos de la operación por categoría."
        actions={
          can(session.permissions, "EXPENSE", "WRITE") && (
            <CreateButton href={`/expenses/${kind}/new`} label={`Registrar ${EXPENSE_KIND_INFO[kind].noun}`} />
          )
        }
      />
      <TabLinks
        label="Categorías de gastos"
        active={kind}
        tabs={EXPENSE_KINDS.map((value) => ({ value, label: EXPENSE_KIND_INFO[value].label, href: `/expenses/${value}` }))}
      />
      <FilterBar fields={fields} />
      <Suspense key={paramsKey(query)} fallback={<ResultsSkeleton />}>
        <ExpensesResults kind={kind} params={query} />
      </Suspense>
    </>
  );
}

async function ExpensesResults({ kind, params }: { kind: ExpenseKind; params: SearchParams }) {
  const page = getPage(params);
  const path = `/expenses/${kind}`;
  const data = await listExpenses(kind, {
    date: getDateParam(params, "date"),
    vehicleId: kind === "fuel" ? getIdParam(params, "vehicleId") : undefined,
    driverId: kind === "fuel" ? getIdParam(params, "driverId") : undefined,
    maintenanceId: kind === "maintenance" ? getIdParam(params, "maintenanceId") : undefined,
    employeeId: kind === "salary" ? getIdParam(params, "employeeId") : undefined,
    incidentId: kind === "incident" ? getIdParam(params, "incidentId") : undefined,
    page,
    pageSize: PAGE_SIZE,
  });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(expense) => expense.id}
        rowHref={(expense) => `${path}/${expense.id}`}
        caption={`Gastos: ${EXPENSE_KIND_INFO[kind].label}`}
        columns={[
          { key: "date", header: "Fecha", primary: true, cell: (expense) => <DateText value={expense.date} /> },
          { key: "amount", header: "Monto", className: "text-right", cell: (expense) => <Money value={expense.amount} /> },
        ]}
        empty={
          <ListEmpty
            filtered={hasFilters(params, ["date", ...REFERENCE_FILTERS])}
            icon={Receipt}
            noun={`gastos registrados en ${EXPENSE_KIND_INFO[kind].label.toLowerCase()}`}
          />
        }
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={path} searchParams={params} />
    </>
  );
}
