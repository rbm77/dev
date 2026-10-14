import { Suspense } from "react";
import type { Metadata } from "next";
import { Users } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge } from "@/components/shared/status-badge";
import { getRoleOptions, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listUsers } from "@/features/users/api";
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

export const metadata: Metadata = { title: "Usuarios" };

const PATH = "/settings/users";
const FILTERS = ["name", "lastName", "identityDocument", "roleId", "isActive"];

export default async function UsersPage({ searchParams }: PageProps<"/settings/users">) {
  const session = await verifySession();
  if (!can(session.permissions, "USER", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const roleOptions = await getRoleOptions();

  return (
    <>
      <PageHeader
        title="Usuarios"
        description="Personas con acceso al sistema."
        actions={can(session.permissions, "USER", "WRITE") && <CreateButton href="/settings/users/new" label="Nuevo usuario" />}
      />
      <FilterBar
        fields={[
          { type: "search", name: "name", label: "Buscar por nombre", maxLength: LIMITS.personalData.name },
          { type: "text", name: "lastName", label: "Apellidos", maxLength: LIMITS.personalData.lastName },
          {
            type: "text",
            name: "identityDocument",
            label: "Identificación exacta",
            maxLength: LIMITS.personalData.identityDocument,
          },
          { type: "select", name: "roleId", label: "Rol", options: roleOptions },
          { type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <UsersResults params={params} />
      </Suspense>
    </>
  );
}

async function UsersResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const [data, roles] = await Promise.all([
    listUsers({
      name: getParam(params, "name", LIMITS.personalData.name),
      lastName: getParam(params, "lastName", LIMITS.personalData.lastName),
      identityDocument: getParam(params, "identityDocument", LIMITS.personalData.identityDocument),
      roleId: getIdParam(params, "roleId"),
      isActive: getBoolParam(params, "isActive"),
      page,
      pageSize: PAGE_SIZE,
    }),
    getRoleOptions().then(toLabelMap),
  ]);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(user) => user.id}
        rowHref={(user) => `${PATH}/${user.id}`}
        caption="Usuarios"
        columns={[
          { key: "name", header: "Nombre", primary: true, cell: (user) => fullName(user.name, user.lastName) },
          { key: "username", header: "Usuario", cell: (user) => user.username ?? "—" },
          { key: "role", header: "Rol", cell: (user) => labelOf(roles, user.roleId) },
          { key: "identity", header: "Identificación", cell: (user) => user.identityDocument ?? "—" },
          { key: "status", header: "Estado", cell: (user) => <ActiveBadge active={user.isActive} /> },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, FILTERS)} icon={Users} noun="usuarios registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
