import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { CalendarClock } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DeleteButton } from "@/components/shared/delete-button";
import { EmptyState } from "@/components/shared/empty-state";
import { DateText } from "@/components/shared/formatted";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { deletePaymentPeriodRequest } from "@/features/payment-periods/actions";
import { getPaymentPeriodRequest, listPaymentPeriods } from "@/features/payment-periods/api";
import { PaymentPeriodRequestForm } from "@/features/payment-periods/components/payment-period-request-form";
import { formatMoney } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getPage } from "@/lib/search-params";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Configuración de cobro" };

const PERIODS_PAGE_SIZE = 24;

export default async function PaymentPeriodRequestPage({ params, searchParams }: PageProps<"/payment-periods/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT_PERIOD", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const query = await searchParams;
  const page = getPage(query);
  const [request, periods] = await Promise.all([getPaymentPeriodRequest(id), listPaymentPeriods(id, page, PERIODS_PAGE_SIZE)]);
  if (!request) {
    notFound();
  }
  const canWrite = can(session.permissions, "PAYMENT_PERIOD", "WRITE");

  return (
    <>
      <PageHeader
        title={`Configuración #${request.id}`}
        description={`${formatMoney(request.amount)} cada ${request.daysToNextPayment} días`}
        backHref="/payment-periods"
        backLabel="Periodos de cobro"
        actions={
          canWrite && (
            <DeleteButton
              action={deletePaymentPeriodRequest.bind(null, id)}
              entityLabel="esta configuración de cobro"
              redirectTo="/payment-periods"
            />
          )
        }
      />
      <div className="flex flex-col gap-6">
        <PaymentPeriodRequestForm request={request} readOnly={!canWrite} />

        <Card className="gap-4">
          <CardHeader>
            <CardTitle className="text-base">Fechas de cobro generadas</CardTitle>
          </CardHeader>
          <CardContent>
            {periods.items.length === 0 ? (
              <EmptyState
                icon={CalendarClock}
                title="Aún no hay fechas generadas"
                description="El servicio de programación las genera automáticamente a partir de la fecha de inicio."
                className="py-8"
              />
            ) : (
              <ul className="grid grid-cols-2 gap-2 sm:grid-cols-3 lg:grid-cols-4">
                {periods.items.map((period) => (
                  <li key={period.id} className="rounded-lg border bg-muted/40 px-3 py-2 text-sm">
                    <DateText value={period.paymentDate} />
                  </li>
                ))}
              </ul>
            )}
            <Pagination
              page={page}
              pageSize={PERIODS_PAGE_SIZE}
              totalCount={periods.totalCount}
              pathname={`/payment-periods/${id}`}
              searchParams={query}
            />
          </CardContent>
        </Card>
      </div>
    </>
  );
}
