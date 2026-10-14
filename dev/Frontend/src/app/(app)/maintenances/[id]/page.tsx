import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { Receipt } from "lucide-react";
import { Button } from "@/components/ui/button";
import { CompleteButton } from "@/components/shared/complete-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { StatusBadge } from "@/components/shared/status-badge";
import { getVehicleOptions, getVehiclePlates, labelOf } from "@/features/catalogs/lookups";
import { completeMaintenance, deleteMaintenance } from "@/features/maintenances/actions";
import { getMaintenance } from "@/features/maintenances/api";
import { MaintenanceForm } from "@/features/maintenances/components/maintenance-form";
import { formatDate } from "@/lib/dates";
import { maintenanceTypeLabel } from "@/lib/enums";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Mantenimiento" };

export default async function MaintenancePage({ params }: PageProps<"/maintenances/[id]">) {
  const session = await verifySession();
  const permissions = session.permissions;
  if (!can(permissions, "MAINTENANCE", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [maintenance, vehicleOptions, plates] = await Promise.all([getMaintenance(id), getVehicleOptions(), getVehiclePlates()]);
  if (!maintenance) {
    notFound();
  }
  const canWrite = can(permissions, "MAINTENANCE", "WRITE");
  const completed = Boolean(maintenance.completedDate);
  const title = `${maintenanceTypeLabel(maintenance.type)} · ${labelOf(plates, maintenance.vehicleId)}`;

  return (
    <>
      <PageHeader
        title={title}
        description={completed ? `Realizado el ${formatDate(maintenance.completedDate)}` : `Programado para el ${formatDate(maintenance.scheduledDate)}`}
        backHref={completed ? "/maintenances?status=completed" : "/maintenances"}
        backLabel="Mantenimientos"
        actions={
          <>
            <StatusBadge tone={completed ? "success" : "warning"}>{completed ? "Completado" : "Pendiente"}</StatusBadge>
            {can(permissions, "EXPENSE", "WRITE") && (
              <Button asChild variant="outline">
                <Link href={`/expenses/maintenance/new?maintenanceId=${id}`}>
                  <Receipt aria-hidden />
                  Registrar gasto
                </Link>
              </Button>
            )}
            {canWrite && !completed && (
              <CompleteButton
                action={completeMaintenance.bind(null, id)}
                title="Completar mantenimiento"
                description="Indica la fecha en que se realizó el mantenimiento."
              />
            )}
            {canWrite && (
              <DeleteButton action={deleteMaintenance.bind(null, id)} entityLabel="este mantenimiento" redirectTo="/maintenances" />
            )}
          </>
        }
      />
      <MaintenanceForm maintenance={maintenance} vehicleOptions={vehicleOptions} readOnly={!canWrite} />
    </>
  );
}
