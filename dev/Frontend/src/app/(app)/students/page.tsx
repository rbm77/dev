import { Suspense } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import { GraduationCap, QrCode } from "lucide-react";
import { Button } from "@/components/ui/button";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge } from "@/components/shared/status-badge";
import { getGradeOptions, getRouteOptions, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listStudents } from "@/features/students/api";
import { LIMITS } from "@/lib/db-limits";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import {
  getBoolParam,
  getIdParam,
  getPage,
  getParam,
  hasFilters,
  PAGE_SIZE,
  paramsKey,
  type SearchParams,
} from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Estudiantes" };

const PATH = "/students";
const FILTERS = ["name", "lastName", "identityDocument", "routeId", "gradeId", "isActive"];

export default async function StudentsPage({ searchParams }: PageProps<"/students">) {
  const session = await verifySession();
  if (!can(session.permissions, "STUDENT", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const [routeOptions, gradeOptions] = await Promise.all([getRouteOptions(), getGradeOptions()]);

  return (
    <>
      <PageHeader
        title="Estudiantes"
        description="Estudiantes que usan el transporte."
        actions={
          <>
            <Button asChild variant="outline">
              <Link href="/students/qr">
                <QrCode aria-hidden />
                Códigos QR
              </Link>
            </Button>
            {can(session.permissions, "STUDENT", "WRITE") && <CreateButton href="/students/new" label="Nuevo estudiante" />}
          </>
        }
      />
      <FilterBar
        fields={[
          { type: "search", name: "name", label: "Buscar por nombre", maxLength: LIMITS.student.name },
          { type: "text", name: "lastName", label: "Apellidos", maxLength: LIMITS.student.lastName },
          { type: "text", name: "identityDocument", label: "Identificación exacta", maxLength: LIMITS.student.identityDocument },
          { type: "select", name: "routeId", label: "Ruta", options: routeOptions },
          { type: "select", name: "gradeId", label: "Grado", options: gradeOptions },
          { type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <StudentsResults params={params} />
      </Suspense>
    </>
  );
}

async function StudentsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const [data, routes, grades] = await Promise.all([
    listStudents({
      name: getParam(params, "name", LIMITS.student.name),
      lastName: getParam(params, "lastName", LIMITS.student.lastName),
      identityDocument: getParam(params, "identityDocument", LIMITS.student.identityDocument),
      routeId: getIdParam(params, "routeId"),
      gradeId: getIdParam(params, "gradeId"),
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
        rowKey={(student) => student.id}
        rowHref={(student) => `${PATH}/${student.id}`}
        caption="Estudiantes"
        columns={[
          { key: "name", header: "Estudiante", primary: true, cell: (student) => fullName(student.name, student.lastName) },
          { key: "identity", header: "Identificación", cell: (student) => student.identityDocument ?? "—" },
          { key: "route", header: "Ruta", cell: (student) => labelOf(routes, student.routeId) },
          { key: "grade", header: "Grado", cell: (student) => labelOf(grades, student.gradeId) },
          { key: "status", header: "Estado", cell: (student) => <ActiveBadge active={student.isActive} /> },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, FILTERS)} icon={GraduationCap} noun="estudiantes registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
