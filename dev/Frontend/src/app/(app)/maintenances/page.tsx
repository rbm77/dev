import { Suspense } from "react";
import type { Metadata } from "next";
import { Wrench } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { TabLinks } from "@/components/shared/tab-links";
import { getVehicleOptions, getVehiclePlates, labelOf } from "@/features/catalogs/lookups";
import { listCompletedMaintenances, listPendingMaintenances } from "@/features/maintenances/api";
import { MAINTENANCE_TYPES, maintenanceTypeLabel } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { getIdParam, getOptionParam, getPage, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Mantenimientos" };

const PATH = "/maintenances";

export default async function MaintenancesPage({ searchParams }: PageProps<"/maintenances">) {
  const session = await verifySession();
  if (!can(session.permissions, "MAINTENANCE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const status = getOptionParam(params, "status", ["completed"] as const) ?? "pending";

  return (
    <>
      <PageHeader
        title="Mantenimientos"
        description="Mantenimientos programados y realizados de la flota."
        actions={can(session.permissions, "MAINTENANCE", "WRITE") && <CreateButton href="/maintenances/new" label="Programar" />}
      />
      <TabLinks
        label="Estado de los mantenimientos"
        active={status}
        tabs={[
          { value: "pending", label: "Pendientes", href: PATH },
          { value: "completed", label: "Completados", href: `${PATH}?status=completed` },
        ]}
      />
      <FilterBar
        fields={[
          { type: "select", name: "vehicleId", label: "Vehículo", options: await getVehicleOptions() },
          { type: "select", name: "type", label: "Tipo", options: MAINTENANCE_TYPES },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <MaintenancesResults params={params} completed={status === "completed"} />
      </Suspense>
    </>
  );
}

async function MaintenancesResults({ params, completed }: { params: SearchParams; completed: boolean }) {
  const page = getPage(params);
  const filters = {
    vehicleId: getIdParam(params, "vehicleId"),
    type: getIdParam(params, "type"),
    page,
    pageSize: PAGE_SIZE,
  };
  const [data, plates] = await Promise.all([
    completed
      ? listCompletedMaintenances(filters).then((result) => ({
          ...result,
          items: result.items.map((item) => ({ ...item, date: item.completedDate })),
        }))
      : listPendingMaintenances(filters).then((result) => ({
          ...result,
          items: result.items.map((item) => ({ ...item, date: item.scheduledDate })),
        })),
    getVehiclePlates(),
  ]);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        rowHref={(item) => `${PATH}/${item.id}`}
        caption={completed ? "Mantenimientos completados" : "Mantenimientos pendientes"}
        columns={[
          { key: "vehicle", header: "Vehículo", primary: true, cell: (item) => labelOf(plates, item.vehicleId) },
          { key: "type", header: "Tipo", cell: (item) => maintenanceTypeLabel(item.type) },
          { key: "date", header: completed ? "Realizado" : "Programado", cell: (item) => <DateText value={item.date} /> },
        ]}
        empty={
          <ListEmpty
            filtered={hasFilters(params, ["vehicleId", "type"])}
            icon={Wrench}
            noun={completed ? "mantenimientos completados" : "mantenimientos pendientes"}
          />
        }
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
