import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getPaymentPeriodOptions, getStudentName } from "@/features/catalogs/lookups";
import { SpecificExemptionForm } from "@/features/exemptions/components/exemption-forms";
import { can } from "@/lib/permissions";
import { getIdParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nueva exención específica" };

export default async function NewSpecificExemptionPage({ searchParams }: PageProps<"/exemptions/specific/new">) {
  const session = await verifySession();
  if (!can(session.permissions, "SPECIFIC_EXEMPTION", "WRITE")) {
    return <NoAccess />;
  }
  const studentId = getIdParam(await searchParams, "studentId");
  const [periodOptions, studentName] = await Promise.all([
    getPaymentPeriodOptions(),
    studentId ? getStudentName(studentId) : Promise.resolve(undefined),
  ]);

  return (
    <>
      <PageHeader title="Nueva exención específica" backHref="/exemptions/specific" backLabel="Exenciones" />
      <SpecificExemptionForm
        periodOptions={periodOptions}
        presetStudent={studentId && studentName ? { id: studentId, label: studentName } : undefined}
      />
    </>
  );
}
