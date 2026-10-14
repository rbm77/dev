import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getStudentName } from "@/features/catalogs/lookups";
import { PaymentForm } from "@/features/payments/components/payment-form";
import { can } from "@/lib/permissions";
import { getIdParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Registrar pago" };

export default async function NewPaymentPage({ searchParams }: PageProps<"/payments/new">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT", "WRITE")) {
    return <NoAccess />;
  }
  const studentId = getIdParam(await searchParams, "studentId");
  const studentName = studentId ? await getStudentName(studentId) : undefined;

  return (
    <>
      <PageHeader title="Registrar pago" backHref="/payments" backLabel="Pagos" />
      <PaymentForm presetStudent={studentId && studentName ? { id: studentId, label: studentName } : undefined} />
    </>
  );
}
