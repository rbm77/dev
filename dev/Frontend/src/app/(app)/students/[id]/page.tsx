import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { Banknote, Percent } from "lucide-react";
import { Button } from "@/components/ui/button";
import { DeleteButton } from "@/components/shared/delete-button";
import { NoAccess } from "@/components/shared/no-access";
import { PageHeader } from "@/components/shared/page-header";
import { TabLinks, type TabLink } from "@/components/shared/tab-links";
import { getGradeOptions, getRouteOptions, labelOf, toLabelMap } from "@/features/catalogs/lookups";
import { deleteStudent } from "@/features/students/actions";
import { getStudent, listContacts } from "@/features/students/api";
import { ContactsPanel } from "@/features/students/components/contacts-panel";
import { StudentForm } from "@/features/students/components/student-form";
import { fullName } from "@/lib/format";
import { can } from "@/lib/permissions";
import { getOptionParam } from "@/lib/search-params";
import { verifySession } from "@/lib/session";
import { parseId } from "@/lib/validation";

export const metadata: Metadata = { title: "Estudiante" };

export default async function StudentPage({ params, searchParams }: PageProps<"/students/[id]">) {
  const session = await verifySession();
  if (!can(session.permissions, "STUDENT", "READ")) {
    return <NoAccess />;
  }
  const id = parseId((await params).id);
  if (!id) {
    notFound();
  }

  const permissions = session.permissions;
  const canReadContacts = can(permissions, "CONTACT", "READ");
  const tab = canReadContacts ? (getOptionParam(await searchParams, "tab", ["contacts"] as const) ?? "data") : "data";

  const [student, routeOptions, gradeOptions] = await Promise.all([getStudent(id), getRouteOptions(), getGradeOptions()]);
  if (!student) {
    notFound();
  }
  const name = fullName(student.name, student.lastName);
  const subtitle = [labelOf(toLabelMap(routeOptions), student.routeId), labelOf(toLabelMap(gradeOptions), student.gradeId)].join(" · ");

  const tabs: TabLink[] = [{ value: "data", label: "Datos", href: `/students/${id}` }];
  if (canReadContacts) {
    tabs.push({ value: "contacts", label: "Contactos", href: `/students/${id}?tab=contacts` });
  }

  return (
    <>
      <PageHeader
        title={name}
        description={subtitle}
        backHref="/students"
        backLabel="Estudiantes"
        actions={
          <>
            {can(permissions, "PAYMENT", "READ") && (
              <Button asChild variant="outline">
                <Link href={`/payments?studentId=${id}`}>
                  <Banknote aria-hidden />
                  Ver pagos
                </Link>
              </Button>
            )}
            {can(permissions, "PERIODIC_EXEMPTION", "READ") && (
              <Button asChild variant="outline">
                <Link href={`/exemptions/periodic?studentId=${id}`}>
                  <Percent aria-hidden />
                  Ver exenciones
                </Link>
              </Button>
            )}
            {can(permissions, "STUDENT", "WRITE") && (
              <DeleteButton action={deleteStudent.bind(null, id)} entityLabel={`a ${name}`} redirectTo="/students" />
            )}
          </>
        }
      />
      {tabs.length > 1 && <TabLinks tabs={tabs} active={tab} label="Secciones del estudiante" />}
      {tab === "contacts" ? (
        <ContactsPanel studentId={id} contacts={await listContacts(id)} canWrite={can(permissions, "CONTACT", "WRITE")} />
      ) : (
        <StudentForm
          student={student}
          routeOptions={routeOptions}
          gradeOptions={gradeOptions}
          readOnly={!can(permissions, "STUDENT", "WRITE")}
        />
      )}
    </>
  );
}
