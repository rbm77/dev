import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { deleteVacation } from "@/features/vacations/actions";
import { getVacation } from "@/features/vacations/api";
import { VacationForm } from "@/features/vacations/components/vacation-form";
import { formatDate } from "@/lib/dates";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Vacaciones" };

export default async function VacationPage({ params }: PageProps<"/vacations/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "VACATION", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const vacation = await getVacation(id);
  if (!vacation) {
    notFound();
  }
  const canWrite = can(session.permissions, "VACATION", "WRITE");

  return (
    <>
      <PageHeader
        title={vacation.description ?? "Vacaciones"}
        description={`Del ${formatDate(vacation.startDate)} al ${formatDate(vacation.endDate)}`}
        backHref="/vacations"
        backLabel="Vacaciones"
        actions={canWrite && <DeleteButton action={deleteVacation.bind(null, id)} entityLabel="este periodo de vacaciones" redirectTo="/vacations" />}
      />
      <VacationForm vacation={vacation} readOnly={!canWrite} />
    </>
  );
}
