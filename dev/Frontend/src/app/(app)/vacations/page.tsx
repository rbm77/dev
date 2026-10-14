import { Suspense } from "react";
import type { Metadata } from "next";
import { CalendarOff } from "lucide-react";
import { CreateButton } from "@/components/shared/create-button";
import { EntityList } from "@/components/shared/entity-list";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { listVacations } from "@/features/vacations/api";
import { can } from "@/lib/permissions";
import { getPage, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Vacaciones" };

const PATH = "/vacations";

export default async function VacationsPage({ searchParams }: PageProps<"/vacations">) {
  const session = await verifySession();
  if (!can(session.permissions, "VACATION", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;

  return (
    <>
      <PageHeader
        title="Vacaciones"
        description="Periodos sin servicio: sus fechas de cobro no se cobran."
        actions={can(session.permissions, "VACATION", "WRITE") && <CreateButton href={`${PATH}/new`} label="Registrar vacaciones" />}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <VacationsResults params={params} />
      </Suspense>
    </>
  );
}

async function VacationsResults({ params }: { params: SearchParams }) {
  const page = getPage(params);
  const data = await listVacations(page, PAGE_SIZE);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(vacation) => vacation.id}
        rowHref={(vacation) => `${PATH}/${vacation.id}`}
        caption="Vacaciones"
        columns={[
          { key: "start", header: "Desde", primary: true, cell: (vacation) => <DateText value={vacation.startDate} /> },
          { key: "end", header: "Hasta", cell: (vacation) => <DateText value={vacation.endDate} /> },
        ]}
        empty={<ListEmpty filtered={false} icon={CalendarOff} noun="vacaciones registradas" />}
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
