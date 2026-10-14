import { Suspense } from "react";
import type { Metadata } from "next";
import { Inbox } from "lucide-react";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText, Money } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { StatusBadge } from "@/components/shared/status-badge";
import { getStudentNames, labelOf } from "@/features/catalogs/lookups";
import { listPaymentRequests } from "@/features/payments/api";
import { PaymentRequestActions } from "@/features/payments/components/payment-request-actions";
import { LIMITS } from "@/lib/db-limits";
import { can } from "@/lib/permissions";
import {
  getBoolParam,
  getDateParam,
  getPage,
  getParam,
  hasFilters,
  PAGE_SIZE,
  paramsKey,
  type SearchParams,
} from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Solicitudes de pago" };

const PATH = "/payment-requests";
const STATUS_OPTIONS = [
  { value: "false", label: "Por validar" },
  { value: "true", label: "Validadas" },
];

export default async function PaymentRequestsPage({ searchParams }: PageProps<"/payment-requests">) {
  const session = await verifySession();
  if (!can(session.permissions, "PAYMENT_REQUEST", "READ")) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const canApprove = can(session.permissions, "PAYMENT_APPROVAL", "WRITE");

  return (
    <>
      <PageHeader
        title="Solicitudes de pago"
        description="Comprobantes recibidos por SMS, correo o WhatsApp. Valídalos y apruébalos para registrar el pago."
      />
      <FilterBar
        fields={[
          {
            type: "search",
            name: "receiptReference",
            label: "Referencia exacta",
            maxLength: LIMITS.payment.receiptReference,
          },
          { type: "select", name: "isValidated", label: "Estado", options: STATUS_OPTIONS },
          { type: "date", name: "requestedAt", label: "Recibida el" },
        ]}
      />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        <PaymentRequestsResults params={params} canApprove={canApprove} />
      </Suspense>
    </>
  );
}

async function PaymentRequestsResults({ params, canApprove }: { params: SearchParams; canApprove: boolean }) {
  const page = getPage(params);
  const data = await listPaymentRequests({
    requestedAt: getDateParam(params, "requestedAt"),
    isValidated: getBoolParam(params, "isValidated"),
    receiptReference: getParam(params, "receiptReference", LIMITS.payment.receiptReference),
    page,
    pageSize: PAGE_SIZE,
  });
  const students = await getStudentNames(data.items.map((request) => request.studentId));

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(request) => request.id}
        caption="Solicitudes de pago"
        columns={[
          { key: "student", header: "Estudiante", primary: true, cell: (request) => labelOf(students, request.studentId) },
          { key: "amount", header: "Monto", className: "text-right", cell: (request) => <Money value={request.amount} /> },
          { key: "date", header: "Fecha del pago", cell: (request) => <DateText value={request.date} /> },
          { key: "reference", header: "Referencia", cell: (request) => request.receiptReference || "—" },
          { key: "requestedAt", header: "Recibida", hideOnMobile: true, cell: (request) => <DateText value={request.requestedAt} /> },
          {
            key: "status",
            header: "Estado",
            cell: (request) =>
              request.isValidated ? (
                <StatusBadge tone="success">Validada</StatusBadge>
              ) : (
                <StatusBadge tone="warning">Por validar</StatusBadge>
              ),
          },
        ]}
        rowActions={canApprove ? (request) => <PaymentRequestActions request={request} /> : undefined}
        mobileActions="footer"
        empty={
          <ListEmpty
            filtered={hasFilters(params, ["requestedAt", "isValidated", "receiptReference"])}
            icon={Inbox}
            noun="solicitudes de pago"
          />
        }
      />
      <Pagination page={page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
