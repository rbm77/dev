import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getRoleOptions } from "@/features/catalogs/lookups";
import { deleteUser } from "@/features/users/actions";
import { getUser } from "@/features/users/api";
import { UserForm } from "@/features/users/components/user-form";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Usuario" };

export default async function UserPage({ params }: PageProps<"/settings/users/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "USER", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const [user, roleOptions] = await Promise.all([getUser(id), getRoleOptions()]);
  if (!user) {
    notFound();
  }
  const canWrite = can(session.permissions, "USER", "WRITE");
  const name = fullName(user.name, user.lastName);

  return (
    <>
      <PageHeader
        title={name}
        description={user.username ? `Usuario: ${user.username}` : undefined}
        backHref="/settings/users"
        backLabel="Usuarios"
        actions={
          canWrite && (
            <DeleteButton action={deleteUser.bind(null, user.id)} entityLabel={`el usuario ${name}`} redirectTo="/settings/users" />
          )
        }
      />
      <UserForm user={user} roleOptions={roleOptions} readOnly={!canWrite} />
    </>
  );
}
