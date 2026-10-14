"use client";

import { useRouter } from "next/navigation";
import { SelectField, SwitchField, TextField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import type { Student } from "@/features/catalogs/types";
import { LIMITS } from "@/lib/db-limits";
import { createStudent, updateStudent } from "../actions";
import { studentSchema, type StudentFormValues } from "../schema";

type StudentFormProps = {
  student?: Student;
  routeOptions: Option[];
  gradeOptions: Option[];
  readOnly?: boolean;
};

export function StudentForm({ student, routeOptions, gradeOptions, readOnly = false }: StudentFormProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<StudentFormValues, unknown>({
    schema: studentSchema,
    defaultValues: {
      name: student?.name ?? "",
      lastName: student?.lastName ?? "",
      identityDocument: student?.identityDocument ?? "",
      address: student?.address ?? "",
      routeId: student ? String(student.routeId) : "",
      gradeId: student ? String(student.gradeId) : "",
      isActive: student?.isActive ?? true,
    },
    submit: (values) => (student ? updateStudent(student.id, values) : createStudent(values)),
    successMessage: student ? "Estudiante actualizado." : "Estudiante registrado. Ahora puedes agregar sus contactos.",
    onSuccess: (id) => {
      // After creating, go to the new student's contacts (id comes from the Location header).
      if (!student) {
        router.push(typeof id === "number" ? `/students/${id}?tab=contacts` : "/students");
      }
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="flex min-w-0 flex-col gap-6">
        <FormSection title="Datos del estudiante">
          <TextField control={control} name="name" label="Nombre" required maxLength={LIMITS.student.name} />
          <TextField control={control} name="lastName" label="Apellidos" required maxLength={LIMITS.student.lastName} />
          <TextField control={control} name="identityDocument" label="Identificación" maxLength={LIMITS.student.identityDocument} />
          <TextField control={control} name="address" label="Dirección" maxLength={LIMITS.student.address} />
        </FormSection>
        <FormSection title="Transporte y nivel">
          <SelectField control={control} name="routeId" label="Ruta" required options={routeOptions} />
          <SelectField control={control} name="gradeId" label="Grado" required options={gradeOptions} />
          <SwitchField
            control={control}
            name="isActive"
            label="Estudiante activo"
            description="Los estudiantes inactivos no generan cobros nuevos."
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={student ? "Guardar cambios" : "Registrar estudiante"} pending={pending} cancelHref="/students" />
      )}
    </form>
  );
}
