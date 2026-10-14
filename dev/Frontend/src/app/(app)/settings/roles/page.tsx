import { Suspense } from "react";
import type { Metadata } from "next";
import { KeyRound } from "lucide-react";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { deleteRole } from "@/features/roles/actions";
import { listRoles } from "@/features/roles/api";
import { CreateRoleButton, RoleRowActions } from "@/features/roles/components/role-dialog";
import { LIMITS } from "@/lib/db-limits";
import { can } from "@/lib/permissions";
import { getPage, getParam, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Roles y permisos" };

const PATH = "/settings/roles";

export default async function RolesPage({ searchParams }: PageProps<"/settings/roles">) {
  const session = await verifySession();
  if (!can(session.permissions, "ROLE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const canWrite = can(session.permissions, "ROLE", "WRITE");

  return (
    <>
      <PageHeader
        title="Roles y permisos"
        description="Cada usuario tiene un rol que define a qué secciones puede acceder."
        actions={canWrite && <CreateRoleButton />}
      />
      <FilterBar
        fields={[{ type: "search", name: "description", label: "Buscar rol", maxLength: LIMITS.role.description }]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <RolesResults params={params} canWrite={canWrite} />
      </Suspense>
    </>
  );
}

async function RolesResults({ params, canWrite }: { params: SearchParams; canWrite: boolean }) {
  const page = getPage(params);
  const data = await listRoles({ description: getParam(params, "description"), page, pageSize: PAGE_SIZE });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(role) => role.id}
        caption="Roles"
        columns={[{ key: "description", header: "Rol", primary: true, cell: (role) => role.description ?? `#${role.id}` }]}
        rowActions={(role) => (
          <RoleRowActions role={role} canWrite={canWrite} deleteAction={deleteRole.bind(null, role.id)} />
        )}
        empty={<ListEmpty filtered={hasFilters(params, ["description"])} icon={KeyRound} noun="roles registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
