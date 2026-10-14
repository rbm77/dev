import { Suspense } from "react";
import type { Metadata } from "next";
import { TriangleAlert } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { getDriverOptions, getVehicleOptions, getVehiclePlates, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { listIncidents } from "@/features/incidents/api";
import { INCIDENT_TYPES, incidentTypeLabel } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { getIdParam, getPage, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Incidentes" };

const PATH = "/incidents";
const FILTERS = ["vehicleId", "driverId", "type"];

export default async function IncidentsPage({ searchParams }: PageProps<"/incidents">) {
  const session = await verifySession();
  if (!can(session.permissions, "INCIDENT", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const [vehicleOptions, driverOptions] = await Promise.all([getVehicleOptions(), getDriverOptions()]);

  return (
    <>
      <PageHeader
        title="Incidentes"
        description="Accidentes, averías y otros eventos de la operación."
        actions={can(session.permissions, "INCIDENT", "WRITE") && <CreateButton href="/incidents/new" label="Registrar incidente" />}
      />
      <FilterBar
        fields={[
          { type: "select", name: "vehicleId", label: "Vehículo", options: vehicleOptions },
          { type: "select", name: "driverId", label: "Conductor", options: driverOptions },
          { type: "select", name: "type", label: "Tipo", options: INCIDENT_TYPES },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <IncidentsResults params={params} />
      </Suspense>
    </>
  );
}

async function IncidentsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const [data, plates, drivers] = await Promise.all([
    listIncidents({
      vehicleId: getIdParam(params, "vehicleId"),
      driverId: getIdParam(params, "driverId"),
      type: getIdParam(params, "type"),
      page,
      pageSize: PAGE_SIZE,
    }),
    getVehiclePlates(),
    getDriverOptions().then(toLabelMap),
  ]);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(incident) => incident.id}
        rowHref={(incident) => `${PATH}/${incident.id}`}
        caption="Incidentes"
        columns={[
          { key: "type", header: "Incidente", primary: true, cell: (incident) => incidentTypeLabel(incident.type) },
          { key: "date", header: "Fecha", cell: (incident) => <DateText value={incident.date} /> },
          { key: "vehicle", header: "Vehículo", cell: (incident) => labelOf(plates, incident.vehicleId) },
          { key: "driver", header: "Conductor", cell: (incident) => labelOf(drivers, incident.driverId) },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, FILTERS)} icon={TriangleAlert} noun="incidentes registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
