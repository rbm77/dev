import { Suspense } from "react";
import type { Metadata } from "next";
import { Percent } from "lucide-react";
import { ActiveFilter } from "@/components/shared/active-filter";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { TabLinks } from "@/components/shared/tab-links";
import { getPaymentPeriodOptions, getStudentName, getStudentNames, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listSpecificExemptions } from "@/features/exemptions/api";
import { exemptionTabs } from "@/features/exemptions/tabs";
import { can } from "@/lib/permissions";
import { getIdParam, getPage, hasFilters, hrefWith, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Exenciones específicas" };

const PATH = "/exemptions/specific";

export default async function SpecificExemptionsPage({ searchParams }: PageProps<"/exemptions/specific">) {
  const session = await verifySession();
  if (!can(session.permissions, "SPECIFIC_EXEMPTION", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const studentId = getIdParam(params, "studentId");
  const [periodOptions, studentName] = await Promise.all([
    getPaymentPeriodOptions(),
    studentId ? getStudentName(studentId) : Promise.resolve(undefined),
  ]);
  const tabs = exemptionTabs(session.permissions, studentId);

  return (
    <>
      <PageHeader
        title="Exenciones"
        description="Descuentos sobre los cobros de los estudiantes."
        actions={
          can(session.permissions, "SPECIFIC_EXEMPTION", "WRITE") && (
            <CreateButton href={studentId ? `${PATH}/new?studentId=${studentId}` : `${PATH}/new`} label="Nueva exención" />
          )
        }
      />
      {tabs.length > 1 && <TabLinks tabs={tabs} active="specific" label="Tipos de exención" />}
      {studentId && (
        <ActiveFilter
          label="Estudiante"
          value={studentName ?? `#${studentId}`}
          clearHref={hrefWith(PATH, params, { studentId: undefined, page: undefined })}
        />
      )}
      <FilterBar fields={[{ type: "select", name: "paymentPeriodId", label: "Fecha de cobro", options: periodOptions }]} />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <SpecificExemptionsResults params={params} />
      </Suspense>
    </>
  );
}

async function SpecificExemptionsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const [data, periods] = await Promise.all([
    listSpecificExemptions({
      studentId: getIdParam(params, "studentId"),
      paymentPeriodId: getIdParam(params, "paymentPeriodId"),
      page,
      pageSize: PAGE_SIZE,
    }),
    getPaymentPeriodOptions().then(toLabelMap),
  ]);
  const students = await getStudentNames(data.items.map((item) => item.studentId));

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        rowHref={(item) => `${PATH}/${item.id}`}
        caption="Exenciones específicas"
        columns={[
          { key: "student", header: "Estudiante", primary: true, cell: (item) => labelOf(students, item.studentId) },
          { key: "period", header: "Fecha de cobro", cell: (item) => labelOf(periods, item.paymentPeriodId) },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, ["studentId", "paymentPeriodId"])} icon={Percent} noun="exenciones específicas" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
