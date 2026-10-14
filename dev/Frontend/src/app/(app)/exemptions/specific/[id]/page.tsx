import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getPaymentPeriodOptions, getStudentName } from "@/features/catalogs/lookups";
import { deleteSpecificExemption } from "@/features/exemptions/actions";
import { getSpecificExemption } from "@/features/exemptions/api";
import { SpecificExemptionForm } from "@/features/exemptions/components/exemption-forms";
import { formatPercent } from "@/lib/format";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Exención específica" };

export default async function SpecificExemptionPage({ params }: PageProps<"/exemptions/specific/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "SPECIFIC_EXEMPTION", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [exemption, periodOptions] = await Promise.all([getSpecificExemption(id), getPaymentPeriodOptions()]);
  if (!exemption) {
    notFound();
  }
  const studentName = await getStudentName(exemption.studentId);
  const canWrite = can(session.permissions, "SPECIFIC_EXEMPTION", "WRITE");

  return (
    <>
      <PageHeader
        title={`${formatPercent(exemption.percentage)} · ${studentName ?? `Estudiante #${exemption.studentId}`}`}
        description={exemption.description}
        backHref="/exemptions/specific"
        backLabel="Exenciones"
        actions={
          canWrite && (
            <DeleteButton action={deleteSpecificExemption.bind(null, id)} entityLabel="esta exención" redirectTo="/exemptions/specific" />
          )
        }
      />
      <SpecificExemptionForm exemption={exemption} studentLabel={studentName} periodOptions={periodOptions} readOnly={!canWrite} />
    </>
  );
}
