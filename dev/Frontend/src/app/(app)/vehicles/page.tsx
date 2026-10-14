import { Suspense } from "react";
import type { Metadata } from "next";
import { CarFront } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge } from "@/components/shared/status-badge";
import { listVehicles } from "@/features/vehicles/api";
import { LIMITS } from "@/lib/db-limits";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { getBoolParam, getPage, getParam, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Vehículos" };

const PATH = "/vehicles";
const FILTERS = ["licensePlate", "make", "model", "isActive"];

export default async function VehiclesPage({ searchParams }: PageProps<"/vehicles">) {
  const session = await verifySession();
  if (!can(session.permissions, "VEHICLE", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;

  return (
    <>
      <PageHeader
        title="Vehículos"
        description="Flota de buses y vehículos de la empresa."
        actions={can(session.permissions, "VEHICLE", "WRITE") && <CreateButton href="/vehicles/new" label="Nuevo vehículo" />}
      />
      <FilterBar
        fields={[
          { type: "search", name: "licensePlate", label: "Placa exacta", maxLength: LIMITS.vehicle.licensePlate },
          { type: "text", name: "make", label: "Marca", maxLength: LIMITS.vehicle.make },
          { type: "text", name: "model", label: "Modelo", maxLength: LIMITS.vehicle.model },
          { type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <VehiclesResults params={params} />
      </Suspense>
    </>
  );
}

async function VehiclesResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listVehicles({
    licensePlate: getParam(params, "licensePlate", LIMITS.vehicle.licensePlate)?.toUpperCase(),
    make: getParam(params, "make", LIMITS.vehicle.make),
    model: getParam(params, "model", LIMITS.vehicle.model),
    isActive: getBoolParam(params, "isActive"),
    page,
    pageSize: PAGE_SIZE,
  });

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(vehicle) => vehicle.id}
        rowHref={(vehicle) => `${PATH}/${vehicle.id}`}
        caption="Vehículos"
        columns={[
          { key: "plate", header: "Placa", primary: true, cell: (vehicle) => vehicle.licensePlate ?? `#${vehicle.id}` },
          { key: "model", header: "Marca y modelo", cell: (vehicle) => [vehicle.make, vehicle.model].filter(Boolean).join(" ") || "—" },
          { key: "status", header: "Estado", cell: (vehicle) => <ActiveBadge active={vehicle.isActive} /> },
        ]}
        empty={<ListEmpty filtered={hasFilters(params, FILTERS)} icon={CarFront} noun="vehículos registrados" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
