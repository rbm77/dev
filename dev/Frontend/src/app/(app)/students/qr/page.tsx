import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { QrGenerator } from "@/features/students/components/qr-generator";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Códigos QR" };

export default async function StudentQrPage() {
  const session = await verifySession();
  if (!can(session.permissions, "STUDENT", "READ")) {
    return <NoAccess />;
  }

  return (
    <>
      <div className="print:hidden">
        <PageHeader
          title="Códigos QR de estudiantes"
          description="Genera códigos para identificar a cada estudiante al abordar."
          backHref="/students"
          backLabel="Estudiantes"
        />
      </div>
      <QrGenerator />
    </>
  );
}
