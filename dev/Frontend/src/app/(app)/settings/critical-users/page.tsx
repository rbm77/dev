import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { listCriticalProcessUsers } from "@/features/users/api";
import { CriticalUsersEditor } from "@/features/users/components/critical-users-editor";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Usuarios de proceso crítico" };

export default async function CriticalUsersPage() {
  const session = await verifySession();
  if (!can(session.permissions, "USER", "READ")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader
        title="Usuarios de proceso crítico"
        description="Usuarios autorizados para cambios sensibles."
      />
      <CriticalUsersEditor
        initial={await listCriticalProcessUsers()}
        readOnly={!can(session.permissions, "USER", "WRITE")}
      />
    </>
  );
}
