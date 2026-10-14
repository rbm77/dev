import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getVehicleOptions } from "@/features/catalogs/lookups";
import { MaintenanceForm } from "@/features/maintenances/components/maintenance-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Programar mantenimiento" };

export default async function NewMaintenancePage() {
  const session = await verifySession();
  if (!can(session.permissions, "MAINTENANCE", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Programar mantenimiento" backHref="/maintenances" backLabel="Mantenimientos" />
      <MaintenanceForm vehicleOptions={await getVehicleOptions()} />
    </>
  );
}
