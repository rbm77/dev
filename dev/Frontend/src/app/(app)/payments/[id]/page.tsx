import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getStudentName } from "@/features/catalogs/lookups";
import { deletePayment } from "@/features/payments/actions";
import { getPayment } from "@/features/payments/api";
import { PaymentForm } from "@/features/payments/components/payment-form";
import { formatDate } from "@/lib/dates";
import { formatMoney } from "@/lib/format";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Pago" };

export default async function PaymentPage({ params }: PageProps<"/payments/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const payment = await getPayment(id);
  if (!payment) {
    notFound();
  }
  const studentName = await getStudentName(payment.studentId);
  const canWrite = can(session.permissions, "PAYMENT", "WRITE");

  return (
    <>
      <PageHeader
        title={`${formatMoney(payment.amount)} · ${formatDate(payment.date)}`}
        description={studentName ?? `Estudiante #${payment.studentId}`}
        backHref="/payments"
        backLabel="Pagos"
        actions={canWrite && <DeleteButton action={deletePayment.bind(null, id)} entityLabel="este pago" redirectTo="/payments" />}
      />
      <PaymentForm payment={payment} studentLabel={studentName} readOnly={!canWrite} />
    </>
  );
}
