import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getDriverOptions, getVehicleOptions } from "@/features/catalogs/lookups";
import { CustomTransportForm } from "@/features/custom-transports/components/custom-transport-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Programar transporte especial" };

export default async function NewCustomTransportPage() {
  const session = await verifySession();
  if (!can(session.permissions, "CUSTOM_TRANSPORT", "WRITE")) {
    return <NoAccess />;
  }
  const [vehicleOptions, driverOptions] = await Promise.all([getVehicleOptions(), getDriverOptions()]);

  return (
    <>
      <PageHeader title="Programar transporte especial" backHref="/custom-transports" backLabel="Transportes especiales" />
      <CustomTransportForm vehicleOptions={vehicleOptions} driverOptions={driverOptions} />
    </>
  );
}
