import type { Metadata } from "next";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { EmployeeForm } from "@/features/employees/components/employee-form";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export const metadata: Metadata = { title: "Nuevo empleado" };

export default async function NewEmployeePage() {
  const session = await verifySession();
  if (!can(session.permissions, "EMPLOYEE", "WRITE")) {
    return <NoAccess />;
  }

  return (
    <>
      <PageHeader title="Nuevo empleado" backHref="/employees" backLabel="Empleados" />
      <EmployeeForm />
    </>
  );
}
