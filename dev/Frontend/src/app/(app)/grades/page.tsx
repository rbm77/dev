import { Suspense } from "react";
import type { Metadata } from "next";
import { ClipboardCheck } from "lucide-react";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { deleteGrade } from "@/features/grades/actions";
import { listGrades } from "@/features/grades/api";
import { CreateGradeButton, GradeRowActions } from "@/features/grades/components/grade-dialog";
import { LIMITS } from "@/lib/db-limits";
import { can } from "@/lib/permissions";
import { getPage, getParam, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Grados" };

const PATH = "/grades";

export default async function GradesPage({ searchParams }: PageProps<"/grades">) {
  const session = await verifySession();
  if (!can(session.permissions, "GRADE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const canWrite = can(session.permissions, "GRADE", "WRITE");

  return (
    <>
      <PageHeader title="Grados" description="Niveles escolares de los estudiantes." actions={canWrite && <CreateGradeButton />} />
      <FilterBar fields={[{ type: "search", name: "description", label: "Buscar grado", maxLength: LIMITS.grade.description }]} />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <GradesResults params={params} canWrite={canWrite} />
      </Suspense>
    </>
  );
}

async function GradesResults({ params, canWrite }: { params: SearchParams; canWrite: boolean }) {
  const page = getPage(params);
  const data = await listGrades({
    description: getParam(params, "description", LIMITS.grade.description),
    page,
    pageSize: PAGE_SIZE,
  });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(grade) => grade.id}
        caption="Grados"
        columns={[{ key: "description", header: "Grado", primary: true, cell: (grade) => grade.description ?? `#${grade.id}` }]}
        rowActions={
          canWrite
            ? (grade) => <GradeRowActions grade={grade} deleteAction={deleteGrade.bind(null, grade.id)} />
            : undefined
        }
        empty={<ListEmpty filtered={hasFilters(params, ["description"])} icon={ClipboardCheck} noun="grados registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
