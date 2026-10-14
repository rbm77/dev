import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getDriverOptions, getVehicleOptions } from "@/features/catalogs/lookups";
import { IncidentForm } from "@/features/incidents/components/incident-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Registrar incidente" };

export default async function NewIncidentPage() {
  const session = await verifySession();
  if (!can(session.permissions, "INCIDENT", "WRITE")) {
    return <NoAccess />;
  }
  const [vehicleOptions, driverOptions] = await Promise.all([getVehicleOptions(), getDriverOptions()]);

  return (
    <>
      <PageHeader title="Registrar incidente" backHref="/incidents" backLabel="Incidentes" />
      <IncidentForm vehicleOptions={vehicleOptions} driverOptions={driverOptions} />
    </>
  );
}
