import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { PaymentPeriodRequestForm } from "@/features/payment-periods/components/payment-period-request-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nueva configuración de cobro" };

export default async function NewPaymentPeriodRequestPage() {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT_PERIOD", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Nueva configuración de cobro" backHref="/payment-periods" backLabel="Periodos de cobro" />
      <PaymentPeriodRequestForm />
    </>
  );
}
