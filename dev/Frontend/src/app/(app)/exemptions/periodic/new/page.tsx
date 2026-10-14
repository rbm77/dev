import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getStudentName } from "@/features/catalogs/lookups";
import { PeriodicExemptionForm } from "@/features/exemptions/components/exemption-forms";
import { can } from "@/lib/permissions";
import { getIdParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nueva exención periódica" };

export default async function NewPeriodicExemptionPage({ searchParams }: PageProps<"/exemptions/periodic/new">) {
  const session = await verifySession();
  if (!can(session.permissions, "PERIODIC_EXEMPTION", "WRITE")) {
    return <NoAccess />;
  }
  const studentId = getIdParam(await searchParams, "studentId");
  const studentName = studentId ? await getStudentName(studentId) : undefined;

  return (
    <>
      <PageHeader title="Nueva exención periódica" backHref="/exemptions/periodic" backLabel="Exenciones" />
      <PeriodicExemptionForm presetStudent={studentId && studentName ? { id: studentId, label: studentName } : undefined} />
    </>
  );
}
