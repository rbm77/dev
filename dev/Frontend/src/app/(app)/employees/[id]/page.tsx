import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { Receipt } from "lucide-react";
import { Button } from "@/components/ui/button";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { TabLinks, type TabLink } from "@/components/shared/tab-links";
import { deleteEmployee } from "@/features/employees/actions";
import { getEmployee, listSalaries } from "@/features/employees/api";
import { EmployeeForm } from "@/features/employees/components/employee-form";
import { SalariesPanel } from "@/features/employees/components/salaries-panel";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getOptionParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Empleado" };

export default async function EmployeePage({ params, searchParams }: PageProps<"/employees/[id]">) {
  const session = await verifySession();
  const permissions = session.permissions;
  if (!can(permissions, "EMPLOYEE", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }
  const canReadSalaries = can(permissions, "SALARY", "READ");
  const tab = canReadSalaries ? (getOptionParam(await searchParams, "tab", ["salaries"] as const) ?? "data") : "data";

  const employee = await getEmployee(id);
  if (!employee) {
    notFound();
  }
  const name = fullName(employee.name, employee.lastName);
  const tabs: TabLink[] = [{ value: "data", label: "Datos", href: `/employees/${id}` }];
  if (canReadSalaries) {
    tabs.push({ value: "salaries", label: "Salarios", href: `/employees/${id}?tab=salaries` });
  }

  return (
    <>
      <PageHeader
        title={name}
        description={employee.driverDetails?.licenseNumber ? "Conductor" : "Empleado"}
        backHref="/employees"
        backLabel="Empleados"
        actions={
          <>
            {can(permissions, "EXPENSE", "WRITE") && (
              <Button asChild variant="outline">
                <Link href={`/expenses/salary/new?employeeId=${id}`}>
                  <Receipt aria-hidden />
                  Registrar pago de salario
                </Link>
              </Button>
            )}
            {can(permissions, "EMPLOYEE", "WRITE") && (
              <DeleteButton action={deleteEmployee.bind(null, id)} entityLabel={`a ${name}`} redirectTo="/employees" />
            )}
          </>
        }
      />
      {tabs.length > 1 && <TabLinks tabs={tabs} active={tab} label="Secciones del empleado" />}
      {tab === "salaries" ? (
        <SalariesPanel employeeId={id} salaries={await listSalaries(id)} canWrite={can(permissions, "SALARY", "WRITE")} />
      ) : (
        <EmployeeForm employee={employee} readOnly={!can(permissions, "EMPLOYEE", "WRITE")} />
      )}
    </>
  );
}
