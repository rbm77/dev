import type { Metadata } from "next";
import { Info } from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getRoleOptions } from "@/features/catalogs/lookups";
import { UserForm } from "@/features/users/components/user-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nuevo usuario" };

export default async function NewUserPage() {
  const session = await verifySession();
  if (!can(session.permissions, "USER", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Nuevo usuario" backHref="/settings/users" backLabel="Usuarios" />
      <Alert className="mb-6">
        <Info aria-hidden />
        <AlertDescription>La contraseña inicial la genera el sistema automáticamente.</AlertDescription>
      </Alert>
      <UserForm roleOptions={await getRoleOptions()} />
    </>
  );
}
