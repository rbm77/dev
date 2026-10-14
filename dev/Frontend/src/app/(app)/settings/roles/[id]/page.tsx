import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getRole, getRolePermissions } from "@/features/roles/api";
import { PermissionsEditor } from "@/features/roles/components/permissions-editor";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Permisos del rol" };

export default async function RolePermissionsPage({ params }: PageProps<"/settings/roles/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "ROLE", "READ")) {
    return <NoAccess />;
  }

  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [role, permissions] = await Promise.all([getRole(id), getRolePermissions(id)]);
  if (!role) {
    notFound();
  }

  return (
    <>
      <PageHeader
        title={`Permisos: ${role.description ?? `Rol #${role.id}`}`}
        description="Define qué puede ver y modificar este rol en cada sección."
        backHref="/settings/roles"
        backLabel="Roles"
      />
      <PermissionsEditor roleId={role.id} permissions={permissions} readOnly={!can(session.permissions, "ROLE", "WRITE")} />
    </>
  );
}
