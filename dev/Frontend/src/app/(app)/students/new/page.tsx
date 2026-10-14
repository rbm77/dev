import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { getGradeOptions, getRouteOptions } from "@/features/catalogs/lookups";
import { StudentForm } from "@/features/students/components/student-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nuevo estudiante" };

export default async function NewStudentPage() {
  const session = await verifySession();
  if (!can(session.permissions, "STUDENT", "WRITE")) {
    return <NoAccess />;
  }
  const [routeOptions, gradeOptions] = await Promise.all([getRouteOptions(), getGradeOptions()]);

  return (
    <>
      <PageHeader title="Nuevo estudiante" backHref="/students" backLabel="Estudiantes" />
      <StudentForm routeOptions={routeOptions} gradeOptions={gradeOptions} />
    </>
  );
}
