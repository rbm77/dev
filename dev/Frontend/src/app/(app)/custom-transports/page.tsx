import { Suspense } from "react";
import type { Metadata } from "next";
import { Route } from "lucide-react";
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
import { getDriverOptions, getVehicleOptions, getVehiclePlates, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listCompletedCustomTransports, listPendingCustomTransports } from "@/features/custom-transports/api";
import { can } from "@/lib/permissions";
import { getIdParam, getOptionParam, getPage, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Transportes especiales" };

const PATH = "/custom-transports";

export default async function CustomTransportsPage({ searchParams }: PageProps<"/custom-transports">) {
  const session = await verifySession();
  if (!can(session.permissions, "CUSTOM_TRANSPORT", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const status = getOptionParam(params, "status", ["completed"] as const) ?? "pending";
  const [vehicleOptions, driverOptions] = await Promise.all([getVehicleOptions(), getDriverOptions()]);

  return (
    <>
      <PageHeader
        title="Transportes especiales"
        description="Servicios fuera de las rutas regulares (excursiones, eventos)."
        actions={can(session.permissions, "CUSTOM_TRANSPORT", "WRITE") && <CreateButton href={`${PATH}/new`} label="Programar" />}
      />
      <TabLinks
        label="Estado de los transportes"
        active={status}
        tabs={[
          { value: "pending", label: "Pendientes", href: PATH },
          { value: "completed", label: "Completados", href: `${PATH}?status=completed` },
        ]}
      />
      <FilterBar
        fields={[
          { type: "select", name: "vehicleId", label: "Vehículo", options: vehicleOptions },
          { type: "select", name: "driverId", label: "Conductor", options: driverOptions },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <CustomTransportsResults params={params} completed={status === "completed"} />
      </Suspense>
    </>
  );
}

async function CustomTransportsResults({ params, completed }: { params: SearchParams; completed: boolean }) {
  const page = getPage(params);
  const filters = { vehicleId: getIdParam(params, "vehicleId"), driverId: getIdParam(params, "driverId"), page, pageSize: PAGE_SIZE };
  const [data, plates, drivers] = await Promise.all([
    completed
      ? listCompletedCustomTransports(filters).then((result) => ({
          ...result,
          items: result.items.map((item) => ({ ...item, date: item.completedDate })),
        }))
      : listPendingCustomTransports(filters).then((result) => ({
          ...result,
          items: result.items.map((item) => ({ ...item, date: item.scheduledDate })),
        })),
    getVehiclePlates(),
    getDriverOptions().then(toLabelMap),
  ]);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        rowHref={(item) => `${PATH}/${item.id}`}
        caption={completed ? "Transportes completados" : "Transportes pendientes"}
        columns={[
          { key: "date", header: completed ? "Realizado" : "Programado", primary: true, cell: (item) => <DateText value={item.date} /> },
          { key: "vehicle", header: "Vehículo", cell: (item) => labelOf(plates, item.vehicleId) },
          { key: "driver", header: "Conductor", cell: (item) => labelOf(drivers, item.driverId) },
        ]}
        empty={
          <ListEmpty
            filtered={hasFilters(params, ["vehicleId", "driverId"])}
            icon={Route}
            noun={completed ? "transportes completados" : "transportes pendientes"}
          />
        }
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
