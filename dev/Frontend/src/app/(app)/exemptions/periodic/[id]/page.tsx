import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getStudentName } from "@/features/catalogs/lookups";
import { deletePeriodicExemption } from "@/features/exemptions/actions";
import { getPeriodicExemption } from "@/features/exemptions/api";
import { PeriodicExemptionForm } from "@/features/exemptions/components/exemption-forms";
import { formatDate } from "@/lib/dates";
import { formatPercent } from "@/lib/format";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Exención periódica" };

export default async function PeriodicExemptionPage({ params }: PageProps<"/exemptions/periodic/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "PERIODIC_EXEMPTION", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const exemption = await getPeriodicExemption(id);
  if (!exemption) {
    notFound();
  }
  const studentName = await getStudentName(exemption.studentId);
  const canWrite = can(session.permissions, "PERIODIC_EXEMPTION", "WRITE");

  return (
    <>
      <PageHeader
        title={`${formatPercent(exemption.percentage)} · ${studentName ?? `Estudiante #${exemption.studentId}`}`}
        description={`Del ${formatDate(exemption.startDate)} al ${formatDate(exemption.endDate)}`}
        backHref="/exemptions/periodic"
        backLabel="Exenciones"
        actions={
          canWrite && (
            <DeleteButton action={deletePeriodicExemption.bind(null, id)} entityLabel="esta exención" redirectTo="/exemptions/periodic" />
          )
        }
      />
      <PeriodicExemptionForm exemption={exemption} studentLabel={studentName} readOnly={!canWrite} />
    </>
  );
}
