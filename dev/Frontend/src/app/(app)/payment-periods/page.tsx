import { Suspense } from "react";
import type { Metadata } from "next";
import { CalendarDays } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { listPaymentPeriodRequests } from "@/features/payment-periods/api";
import { can } from "@/lib/permissions";
import { getPage, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Periodos de cobro" };

const PATH = "/payment-periods";

export default async function PaymentPeriodsPage({ searchParams }: PageProps<"/payment-periods">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT_PERIOD", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;

  return (
    <>
      <PageHeader
        title="Periodos de cobro"
        description="Configuraciones que generan las fechas de cobro a los estudiantes."
        actions={
          can(session.permissions, "PAYMENT_PERIOD", "WRITE") && <CreateButton href={`${PATH}/new`} label="Nueva configuración" />
        }
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <PaymentPeriodRequestsResults params={params} />
      </Suspense>
    </>
  );
}

async function PaymentPeriodRequestsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listPaymentPeriodRequests(page, PAGE_SIZE);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(request) => request.id}
        rowHref={(request) => `${PATH}/${request.id}`}
        caption="Configuraciones de cobro"
        columns={[
          { key: "name", header: "Configuración", primary: true, cell: (request) => `Configuración #${request.id}` },
          { key: "start", header: "Inicio", cell: (request) => <DateText value={request.startDate} /> },
        ]}
        empty={<ListEmpty filtered={false} icon={CalendarDays} noun="configuraciones de cobro" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
