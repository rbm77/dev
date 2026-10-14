import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { ActiveBadge } from "@/components/shared/status-badge";
import { getCompany } from "@/features/company/api";
import { CompanyForm } from "@/features/company/components/company-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Empresa" };

export default async function CompanyPage() {
  const session = await verifySession();
  if (!can(session.permissions, "COMPANY", "READ")) {
    return <NoAccess />;
  }

  const company = await getCompany(session.companyId);
  if (!company) {
    notFound();
  }

  return (
    <>
      <PageHeader
        title="Empresa"
        description="Datos de la empresa y configuración de cobros."
        actions={<ActiveBadge active={company.isActive} feminine />}
      />
      <CompanyForm company={company} readOnly={!can(session.permissions, "COMPANY", "WRITE")} />
    </>
  );
}
