import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getOwnUser } from "@/features/users/api";
import { ChangePasswordForm } from "@/features/users/components/change-password-form";
import { ProfileForm } from "@/features/users/components/profile-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Mi perfil" };

export default async function ProfilePage() {
  const session = await verifySession();
  if (!can(session.permissions, "OWN_USER", "READ")) {
    return <NoAccess />;
  }

  const user = await getOwnUser(session.userId);
  if (!user) {
    notFound();
  }
  const canWrite = can(session.permissions, "OWN_USER", "WRITE");

  return (
    <>
      <PageHeader title="Mi perfil" description="Revisa y actualiza tu información personal." />
      <div className="grid items-start gap-6 xl:grid-cols-[1fr_22rem]">
        <ProfileForm user={user} readOnly={!canWrite} />
        {canWrite && <ChangePasswordForm />}
      </div>
    </>
  );
}
