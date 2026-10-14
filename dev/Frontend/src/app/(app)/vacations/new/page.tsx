import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { VacationForm } from "@/features/vacations/components/vacation-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Registrar vacaciones" };

export default async function NewVacationPage() {
  const session = await verifySession();
  if (!can(session.permissions, "VACATION", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Registrar vacaciones" backHref="/vacations" backLabel="Vacaciones" />
      <VacationForm />
    </>
  );
}
