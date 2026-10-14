import { Suspense } from "react";
import type { Metadata } from "next";
import { UsersRound } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge, StatusBadge } from "@/components/shared/status-badge";
import { listEmployees } from "@/features/employees/api";
import { LIMITS } from "@/lib/db-limits";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getBoolParam, getPage, getParam, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Empleados" };

const PATH = "/employees";
const FILTERS = ["name", "lastName", "identityDocument", "isActive"];

export default async function EmployeesPage({ searchParams }: PageProps<"/employees">) {
  const session = await verifySession();
  if (!can(session.permissions, "EMPLOYEE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;

  return (
    <>
      <PageHeader
        title="Empleados"
        description="Personal de la empresa, incluidos los conductores."
        actions={can(session.permissions, "EMPLOYEE", "WRITE") && <CreateButton href="/employees/new" label="Nuevo empleado" />}
      />
      <FilterBar
        fields={[
          { type: "search", name: "name", label: "Buscar por nombre", maxLength: LIMITS.personalData.name },
          { type: "text", name: "lastName", label: "Apellidos", maxLength: LIMITS.personalData.lastName },
          { type: "text", name: "identityDocument", label: "Identificación exacta", maxLength: LIMITS.personalData.identityDocument },
          { type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <EmployeesResults params={params} />
      </Suspense>
    </>
  );
}

async function EmployeesResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listEmployees({
    name: getParam(params, "name", LIMITS.personalData.name),
    lastName: getParam(params, "lastName", LIMITS.personalData.lastName),
    identityDocument: getParam(params, "identityDocument", LIMITS.personalData.identityDocument),
    isActive: getBoolParam(params, "isActive"),
    page,
    pageSize: PAGE_SIZE,
  });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(employee) => employee.id}
        rowHref={(employee) => `${PATH}/${employee.id}`}
        caption="Empleados"
        columns={[
          { key: "name", header: "Empleado", primary: true, cell: (employee) => fullName(employee.name, employee.lastName) },
          { key: "identity", header: "Identificación", cell: (employee) => employee.identityDocument ?? "—" },
          { key: "hireDate", header: "Ingreso", cell: (employee) => <DateText value={employee.hireDate} /> },
          {
            key: "role",
            header: "Puesto",
            cell: (employee) =>
              employee.driverDetails?.licenseNumber ? <StatusBadge tone="info">Conductor</StatusBadge> : "—",
          },
          { key: "status", header: "Estado", cell: (employee) => <ActiveBadge active={employee.isActive} /> },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, FILTERS)} icon={UsersRound} noun="empleados registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
