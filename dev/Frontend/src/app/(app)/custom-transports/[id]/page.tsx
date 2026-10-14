import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { CompleteButton } from "@/components/shared/complete-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { StatusBadge } from "@/components/shared/status-badge";
import { getDriverOptions, getVehicleOptions } from "@/features/catalogs/lookups";
import { completeCustomTransport, deleteCustomTransport } from "@/features/custom-transports/actions";
import { getCustomTransport } from "@/features/custom-transports/api";
import { CustomTransportForm } from "@/features/custom-transports/components/custom-transport-form";
import { formatDate } from "@/lib/dates";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Transporte especial" };

export default async function CustomTransportPage({ params }: PageProps<"/custom-transports/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "CUSTOM_TRANSPORT", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [transport, vehicleOptions, driverOptions] = await Promise.all([
    getCustomTransport(id),
    getVehicleOptions(),
    getDriverOptions(),
  ]);
  if (!transport) {
    notFound();
  }
  const canWrite = can(session.permissions, "CUSTOM_TRANSPORT", "WRITE");
  const completed = Boolean(transport.completedDate);

  return (
    <>
      <PageHeader
        title={transport.description ?? "Transporte especial"}
        description={completed ? `Realizado el ${formatDate(transport.completedDate)}` : `Programado para el ${formatDate(transport.scheduledDate)}`}
        backHref={completed ? "/custom-transports?status=completed" : "/custom-transports"}
        backLabel="Transportes especiales"
        actions={
          <>
            <StatusBadge tone={completed ? "success" : "warning"}>{completed ? "Completado" : "Pendiente"}</StatusBadge>
            {canWrite && !completed && (
              <CompleteButton
                action={completeCustomTransport.bind(null, id)}
                title="Completar transporte"
                description="Indica la fecha en que se realizó el servicio."
              />
            )}
            {canWrite && (
              <DeleteButton action={deleteCustomTransport.bind(null, id)} entityLabel="este transporte" redirectTo="/custom-transports" />
            )}
          </>
        }
      />
      <CustomTransportForm
        transport={transport}
        vehicleOptions={vehicleOptions}
        driverOptions={driverOptions}
        readOnly={!canWrite}
      />
    </>
  );
}
