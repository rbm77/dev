import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { VehicleForm } from "@/features/vehicles/components/vehicle-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nuevo vehículo" };

export default async function NewVehiclePage() {
  const session = await verifySession();
  if (!can(session.permissions, "VEHICLE", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Nuevo vehículo" backHref="/vehicles" backLabel="Vehículos" />
      <VehicleForm />
    </>
  );
}
