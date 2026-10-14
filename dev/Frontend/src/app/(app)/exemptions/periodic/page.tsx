import { Suspense } from "react";
import type { Metadata } from "next";
import { Percent } from "lucide-react";
import { ActiveFilter } from "@/components/shared/active-filter";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { TabLinks } from "@/components/shared/tab-links";
import { getStudentName, getStudentNames, labelOf } from "@/features/catalogs/lookups";
import { listPeriodicExemptions } from "@/features/exemptions/api";
import { exemptionTabs } from "@/features/exemptions/tabs";
import { formatPercent } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getIdParam, getPage, hasFilters, hrefWith, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Exenciones periódicas" };

const PATH = "/exemptions/periodic";

export default async function PeriodicExemptionsPage({ searchParams }: PageProps<"/exemptions/periodic">) {
  const session = await verifySession();
  if (!can(session.permissions, "PERIODIC_EXEMPTION", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const studentId = getIdParam(params, "studentId");
  const studentName = studentId ? await getStudentName(studentId) : undefined;
  const tabs = exemptionTabs(session.permissions, studentId);

  return (
    <>
      <PageHeader
        title="Exenciones"
        description="Descuentos sobre los cobros de los estudiantes."
        actions={
          can(session.permissions, "PERIODIC_EXEMPTION", "WRITE") && (
            <CreateButton href={studentId ? `${PATH}/new?studentId=${studentId}` : `${PATH}/new`} label="Nueva exención" />
          )
        }
      />
      {tabs.length > 1 && <TabLinks tabs={tabs} active="periodic" label="Tipos de exención" />}
      {studentId && (
        <ActiveFilter
          label="Estudiante"
          value={studentName ?? `#${studentId}`}
          clearHref={hrefWith(PATH, params, { studentId: undefined, page: undefined })}
        />
      )}
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <PeriodicExemptionsResults params={params} />
      </Suspense>
    </>
  );
}

async function PeriodicExemptionsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listPeriodicExemptions({ studentId: getIdParam(params, "studentId"), page, pageSize: PAGE_SIZE });
  const students = await getStudentNames(data.items.map((item) => item.studentId));

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        rowHref={(item) => `${PATH}/${item.id}`}
        caption="Exenciones periódicas"
        columns={[
          { key: "student", header: "Estudiante", primary: true, cell: (item) => labelOf(students, item.studentId) },
          { key: "percentage", header: "Descuento", className: "text-right", cell: (item) => formatPercent(item.percentage) },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, ["studentId"])} icon={Percent} noun="exenciones periódicas" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
