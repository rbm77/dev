import { Suspense } from "react";
import type { Metadata } from "next";
import { Banknote } from "lucide-react";
import { ActiveFilter } from "@/components/shared/active-filter";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { getStudentName, getStudentNames, labelOf } from "@/features/catalogs/lookups";
import { listPayments } from "@/features/payments/api";
import { LIMITS } from "@/lib/db-limits";
import { can } from "@/lib/permissions";
import {
  getDateParam,
  getIdParam,
  getPage,
  getParam,
  hasFilters,
  hrefWith,
  PAGE_SIZE,
  paramsKey,
  type SearchParams,
} from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Pagos" };

const PATH = "/payments";

export default async function PaymentsPage({ searchParams }: PageProps<"/payments">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const studentId = getIdParam(params, "studentId");
  const studentName = studentId ? await getStudentName(studentId) : undefined;

  return (
    <>
      <PageHeader
        title="Pagos"
        description="Pagos registrados de los estudiantes."
        actions={
          can(session.permissions, "PAYMENT", "WRITE") && (
            <CreateButton href={studentId ? `${PATH}/new?studentId=${studentId}` : `${PATH}/new`} label="Registrar pago" />
          )
        }
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
          { type: "search", name: "receiptReference", label: "Referencia exacta", maxLength: LIMITS.payment.receiptReference },
          { type: "date", name: "date", label: "Fecha" },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <PaymentsResults params={params} />
      </Suspense>
    </>
  );
}

async function PaymentsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listPayments({
    date: getDateParam(params, "date"),
    studentId: getIdParam(params, "studentId"),
    receiptReference: getParam(params, "receiptReference", LIMITS.payment.receiptReference),
    page,
    pageSize: PAGE_SIZE,
  });
  const students = await getStudentNames(data.items.map((payment) => payment.studentId));

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(payment) => payment.id}
        rowHref={(payment) => `${PATH}/${payment.id}`}
        caption="Pagos"
        columns={[
          { key: "student", header: "Estudiante", primary: true, cell: (payment) => labelOf(students, payment.studentId) },
          { key: "date", header: "Fecha", cell: (payment) => <DateText value={payment.date} /> },
          { key: "reference", header: "Referencia", cell: (payment) => payment.receiptReference ?? "—" },
        ]}
        empty={
          <ListEmpty
            filtered={hasFilters(params, ["date", "studentId", "receiptReference"])}
            icon={Banknote}
            noun="pagos registrados"
          />
        }
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
