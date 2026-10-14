import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { Receipt } from "lucide-react";
import { Button } from "@/components/ui/button";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getDriverOptions, getVehicleOptions } from "@/features/catalogs/lookups";
import { deleteIncident } from "@/features/incidents/actions";
import { getIncident } from "@/features/incidents/api";
import { IncidentForm } from "@/features/incidents/components/incident-form";
import { formatDate } from "@/lib/dates";
import { incidentTypeLabel } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Incidente" };

export default async function IncidentPage({ params }: PageProps<"/incidents/[id]">) {
  const session = await verifySession();
  const permissions = session.permissions;
  if (!can(permissions, "INCIDENT", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [incident, vehicleOptions, driverOptions] = await Promise.all([getIncident(id), getVehicleOptions(), getDriverOptions()]);
  if (!incident) {
    notFound();
  }
  const canWrite = can(permissions, "INCIDENT", "WRITE");

  return (
    <>
      <PageHeader
        title={incidentTypeLabel(incident.type)}
        description={`Ocurrido el ${formatDate(incident.date)}`}
        backHref="/incidents"
        backLabel="Incidentes"
        actions={
          <>
            {can(permissions, "EXPENSE", "WRITE") && (
              <Button asChild variant="outline">
                <Link href={`/expenses/incident/new?incidentId=${id}`}>
                  <Receipt aria-hidden />
                  Registrar gasto
                </Link>
              </Button>
            )}
            {canWrite && <DeleteButton action={deleteIncident.bind(null, id)} entityLabel="este incidente" redirectTo="/incidents" />}
          </>
        }
      />
      <IncidentForm incident={incident} vehicleOptions={vehicleOptions} driverOptions={driverOptions} readOnly={!canWrite} />
    </>
  );
}
