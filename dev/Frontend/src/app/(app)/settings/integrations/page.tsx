import { Suspense } from "react";
import type { Metadata } from "next";
import { Mail, MessageCircle, Send } from "lucide-react";
import { EntityList } from "@/components/shared/entity-list";
import { FilterBar } from "@/components/shared/filter-bar";
import { DateText } from "@/components/shared/formatted";
import { ListEmpty } from "@/components/shared/list-empty";
import { ResultsSkeleton } from "@/components/shared/list-skeleton";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { Pagination } from "@/components/shared/pagination";
import { ActiveBadge } from "@/components/shared/status-badge";
import { TabLinks, type TabLink } from "@/components/shared/tab-links";
import { listEmailAccounts, listEmailSenders, listWhatsAppAccounts } from "@/features/integrations/api";
import {
  CreateEmailAccountButton,
  CreateEmailSenderButton,
  CreateWhatsAppAccountButton,
  EmailAccountRowActions,
  EmailSenderRowActions,
  WhatsAppAccountRowActions,
} from "@/features/integrations/components/integration-dialogs";
import { ACTIVE_FILTER_OPTIONS } from "@/lib/enums";
import { can, type Permission, type Resource } from "@/lib/permissions";
import { getBoolParam, getOptionParam, getPage, hasFilters, PAGE_SIZE, paramsKey, type SearchParams } from "@/lib/search-params";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Integraciones" };

const PATH = "/settings/integrations";
const TABS = [
  { value: "email", label: "Cuentas de correo", resource: "EMAIL_ACCOUNT" },
  { value: "senders", label: "Remitentes", resource: "EMAIL_SENDER" },
  { value: "whatsapp", label: "WhatsApp", resource: "WHATSAPP_ACCOUNT" },
] as const satisfies ReadonlyArray<{ value: string; label: string; resource: Resource }>;

type Tab = (typeof TABS)[number]["value"];

function visibleTabs(permissions: readonly Permission[]) {
  return TABS.filter((tab) => can(permissions, tab.resource, "READ"));
}

export default async function IntegrationsPage({ searchParams }: PageProps<"/settings/integrations">) {
  const session = await verifySession();
  const tabs = visibleTabs(session.permissions);
  if (tabs.length === 0) {
    return <NoAccess />;
  }
  const params = await searchParams;
  const requested = getOptionParam(params, "tab", tabs.map((tab) => tab.value));
  const current = tabs.find((tab) => tab.value === requested) ?? tabs[0];
  const active: Tab = current.value;
  const canWrite = can(session.permissions, current.resource, "WRITE");

  const links: TabLink[] = tabs.map((tab) => ({ value: tab.value, label: tab.label, href: `${PATH}?tab=${tab.value}` }));

  return (
    <>
      <PageHeader
        title="Integraciones"
        description="Canales por los que llegan los comprobantes de pago: correo del banco y WhatsApp."
        actions={
          canWrite &&
          (active === "email" ? (
            <CreateEmailAccountButton />
          ) : active === "senders" ? (
            <CreateEmailSenderButton />
          ) : (
            <CreateWhatsAppAccountButton />
          ))
        }
      />
      {links.length > 1 && <TabLinks tabs={links} active={active} label="Tipos de integración" />}
      <FilterBar fields={[{ type: "select", name: "isActive", label: "Estado", options: ACTIVE_FILTER_OPTIONS }]} />
      <Suspense key={paramsKey(params)} fallback={<ResultsSkeleton />}>
        {active === "email" ? (
          <EmailAccountsResults params={params} canWrite={canWrite} />
        ) : active === "senders" ? (
          <EmailSendersResults params={params} canWrite={canWrite} />
        ) : (
          <WhatsAppAccountsResults params={params} canWrite={canWrite} />
        )}
      </Suspense>
    </>
  );
}

type ResultsProps = { params: SearchParams; canWrite: boolean };

function filters(params: SearchParams) {
  return { isActive: getBoolParam(params, "isActive"), page: getPage(params), pageSize: PAGE_SIZE };
}

async function EmailAccountsResults({ params, canWrite }: ResultsProps) {
  const query = filters(params);
  const data = await listEmailAccounts(query);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        caption="Cuentas de correo"
        columns={[
          { key: "email", header: "Correo", primary: true, cell: (item) => item.emailAddress ?? `#${item.id}` },
          { key: "server", header: "Servidor", cell: (item) => `${item.imapHost ?? "—"}:${item.imapPort}` },
          { key: "checked", header: "Última revisión", cell: (item) => <DateText value={item.lastCheckedAt} /> },
          { key: "status", header: "Estado", cell: (item) => <ActiveBadge active={item.isActive} feminine /> },
        ]}
        rowActions={canWrite ? (item) => <EmailAccountRowActions item={item} /> : undefined}
        empty={<ListEmpty filtered={hasFilters(params, ["isActive"])} icon={Mail} noun="cuentas de correo configuradas" />}
      />
      <Pagination page={query.page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}

async function EmailSendersResults({ params, canWrite }: ResultsProps) {
  const query = filters(params);
  const data = await listEmailSenders(query);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        caption="Remitentes de correo"
        columns={[
          { key: "sender", header: "Remitente", primary: true, cell: (item) => item.senderAddress ?? `#${item.id}` },
          { key: "description", header: "Descripción", cell: (item) => item.description ?? "—" },
          { key: "status", header: "Estado", cell: (item) => <ActiveBadge active={item.isActive} /> },
        ]}
        rowActions={canWrite ? (item) => <EmailSenderRowActions item={item} /> : undefined}
        empty={<ListEmpty filtered={hasFilters(params, ["isActive"])} icon={Send} noun="remitentes registrados" />}
      />
      <Pagination page={query.page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}

async function WhatsAppAccountsResults({ params, canWrite }: ResultsProps) {
  const query = filters(params);
  const data = await listWhatsAppAccounts(query);

  return (
    <>
      <EntityList
        rows={data.items}
        rowKey={(item) => item.id}
        caption="Cuentas de WhatsApp"
        columns={[
          { key: "phone", header: "Phone Number ID", primary: true, cell: (item) => item.phoneNumberId ?? `#${item.id}` },
          { key: "status", header: "Estado", cell: (item) => <ActiveBadge active={item.isActive} feminine /> },
        ]}
        rowActions={canWrite ? (item) => <WhatsAppAccountRowActions item={item} /> : undefined}
        empty={<ListEmpty filtered={hasFilters(params, ["isActive"])} icon={MessageCircle} noun="cuentas de WhatsApp configuradas" />}
      />
      <Pagination page={query.page} pageSize={PAGE_SIZE} totalCount={data.totalCount} pathname={PATH} searchParams={params} />
    </>
  );
}
