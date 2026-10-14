import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { deleteVehicle } from "@/features/vehicles/actions";
import { getVehicle } from "@/features/vehicles/api";
import { VehicleForm } from "@/features/vehicles/components/vehicle-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Vehículo" };

export default async function VehiclePage({ params }: PageProps<"/vehicles/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "VEHICLE", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const vehicle = await getVehicle(id);
  if (!vehicle) {
    notFound();
  }
  const canWrite = can(session.permissions, "VEHICLE", "WRITE");
  const plate = vehicle.licensePlate ?? `#${vehicle.id}`;

  return (
    <>
      <PageHeader
        title={plate}
        description={[vehicle.make, vehicle.model].filter(Boolean).join(" ")}
        backHref="/vehicles"
        backLabel="Vehículos"
        actions={canWrite && <DeleteButton action={deleteVehicle.bind(null, id)} entityLabel={`el vehículo ${plate}`} redirectTo="/vehicles" />}
      />
      <VehicleForm vehicle={vehicle} readOnly={!canWrite} />
    </>
  );
}
