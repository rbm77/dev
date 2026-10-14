import { Suspense } from "react";
import type { Metadata } from "next";
import { HandCoins } from "lucide-react";
import { ActiveFilter } from "@/components/shared/active-filter";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { Money } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { getGradeOptions, getRouteOptions, getStudentName, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listDebtors } from "@/features/payments/api";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getBoolParam, getIdParam, getPage, hasFilters, hrefWith, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Deudores" };

const PATH = "/debtors";

export default async function DebtorsPage({ searchParams }: PageProps<"/debtors">) {
  const session = await verifySession();
  if (!can(session.permissions, "DEBTOR", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const studentId = getIdParam(params, "studentId");
  const [routeOptions, studentName] = await Promise.all([
    getRouteOptions(),
    studentId ? getStudentName(studentId) : Promise.resolve(undefined),
  ]);

  return (
    <>
      <PageHeader
        title="Deudores"
        description="Estudiantes con saldo pendiente, considerando periodos de cobro, exenciones, vacaciones y pagos."
      />
      {studentId && (
        <ActiveFilter
          label="Estudiante"
          value={studentName ?? `#${studentId}`}
          clearHref={hrefWith(PATH, params, { studentId: undefined, page: undefined })}
        />
      )}
      <FilterBar
        fields={[
          { type: "select", name: "routeId", label: "Ruta", options: routeOptions },
          { type: "select", name: "isActive", label: "Estado del estudiante", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <DebtorsResults params={params} canOpenStudent={can(session.permissions, "STUDENT", "READ")} />
      </Suspense>
    </>
  );
}

async function DebtorsResults({ params, canOpenStudent }: { params: SearchParams; canOpenStudent: boolean }) {
  const page = getPage(params);
  const [data, routes, grades] = await Promise.all([
    listDebtors({
      routeId: getIdParam(params, "routeId"),
      studentId: getIdParam(params, "studentId"),
      isActive: getBoolParam(params, "isActive"),
      page,
      pageSize: PAGE_SIZE,
    }),
    getRouteOptions().then(toLabelMap),
    getGradeOptions().then(toLabelMap),
  ]);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(debtor) => debtor.id}
        rowHref={canOpenStudent ? (debtor) => `/students/${debtor.id}` : undefined}
        caption="Deudores"
        columns={[
          { key: "student", header: "Estudiante", primary: true, cell: (debtor) => fullName(debtor.name, debtor.lastName) },
          {
            key: "due",
            header: "Adeudado",
            className: "text-right",
            cell: (debtor) => <Money value={debtor.dueAmount} className="font-semibold text-red-700" />,
          },
          { key: "route", header: "Ruta", cell: (debtor) => labelOf(routes, debtor.routeId) },
          { key: "grade", header: "Grado", cell: (debtor) => labelOf(grades, debtor.gradeId) },
          { key: "periods", header: "Periodos", className: "text-right", hideOnMobile: true, cell: (debtor) => debtor.periodsCount },
          { key: "payments", header: "Pagos", className: "text-right", hideOnMobile: true, cell: (debtor) => debtor.paymentsCount },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, ["routeId", "isActive", "studentId"])} icon={HandCoins} noun="estudiantes con deuda" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
