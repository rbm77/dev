import { Suspense } from "react";
import type { Metadata } from "next";
import { MapIcon } from "lucide-react";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge } from "@/components/shared/status-badge";
import { deleteRoute } from "@/features/routes/actions";
import { listRoutes } from "@/features/routes/api";
import { CreateRouteButton, RouteRowActions } from "@/features/routes/components/route-dialog";
import { LIMITS } from "@/lib/db-limits";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { getBoolParam, getPage, getParam, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Rutas" };

const PATH = "/routes";

export default async function RoutesPage({ searchParams }: PageProps<"/routes">) {
  const session = await verifySession();
  if (!can(session.permissions, "ROUTE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const canWrite = can(session.permissions, "ROUTE", "WRITE");

  return (
    <>
      <PageHeader title="Rutas" description="Recorridos del transporte escolar." actions={canWrite && <CreateRouteButton />} />
      <FilterBar
        fields={[
          { type: "search", name: "name", label: "Buscar ruta", maxLength: LIMITS.route.name },
          { type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <RoutesResults params={params} canWrite={canWrite} />
      </Suspense>
    </>
  );
}

async function RoutesResults({ params, canWrite }: { params: SearchParams; canWrite: boolean }) {
  const page = getPage(params);
  const data = await listRoutes({
    name: getParam(params, "name", LIMITS.route.name),
    isActive: getBoolParam(params, "isActive"),
    page,
    pageSize: PAGE_SIZE,
  });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(route) => route.id}
        caption="Rutas"
        columns={[
          { key: "name", header: "Ruta", primary: true, cell: (route) => route.name ?? `#${route.id}` },
          { key: "description", header: "Descripción", cell: (route) => route.description ?? "—" },
          { key: "status", header: "Estado", cell: (route) => <ActiveBadge active={route.isActive} feminine /> },
        ]}
        rowActions={
          canWrite
            ? (route) => <RouteRowActions route={route} deleteAction={deleteRoute.bind(null, route.id)} />
            : undefined
        }
        empty={<ListEmpty filtered={hasFilters(params, ["name", "isActive"])} icon={MapIcon} noun="rutas registradas" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
