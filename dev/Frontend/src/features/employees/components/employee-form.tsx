"use client";

import { useRouter } from "next/navigation";
import { useWatch } from "react-hook-form";
import { DateField, SwitchField, TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { PersonalDataFields } from "@/components/forms/personal-data-fields";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import type { Employee } from "@/features/catalogs/types";
import { LIMITS } from "@/lib/db-limits";
import { personalDataDefaults } from "@/lib/schemas";
import { createEmployee, updateEmployee } from "../actions";
import { employeeSchema, type EmployeeFormValues } from "../schema";

export function EmployeeForm({ employee, readOnly = false }: { employee?: Employee; readOnly?: boolean }) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<EmployeeFormValues, unknown>({
    schema: employeeSchema,
    defaultValues: {
      ...personalDataDefaults(employee),
      hireDate: employee?.hireDate ?? "",
      isActive: employee?.isActive ?? true,
      isDriver: Boolean(employee?.driverDetails?.licenseNumber),
      licenseNumber: employee?.driverDetails?.licenseNumber ?? "",
      licenseExpiryDate: employee?.driverDetails?.licenseExpiryDate ?? "",
    },
    submit: (values) => (employee ? updateEmployee(employee.id, values) : createEmployee(values)),
    successMessage: employee ? "Empleado actualizado." : "Empleado registrado.",
    onSuccess: (id) => {
      if (!employee) {
        router.push(typeof id === "number" ? `/employees/${id}` : "/employees");
      }
    },
  });
  const isDriver = useWatch({ control, name: "isDriver" });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="flex min-w-0 flex-col gap-6">
        <FormSection title="Datos personales">
          <PersonalDataFields control={control} />
        </FormSection>
        <FormSection title="Datos laborales">
          <DateField control={control} name="hireDate" label="Fecha de ingreso" required />
          <SwitchField control={control} name="isActive" label="Empleado activo" />
          <SwitchField
            control={control}
            name="isDriver"
            label="Es conductor"
            description="Los conductores se pueden asignar a incidentes, combustible y transportes."
            className="md:col-span-2"
          />
          {isDriver && (
            <>
              <TextField
                control={control}
                name="licenseNumber"
                label="Número de licencia"
                required
                maxLength={LIMITS.driver.licenseNumber}
              />
              <DateField control={control} name="licenseExpiryDate" label="Vencimiento de la licencia" required />
            </>
          )}
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={employee ? "Guardar cambios" : "Registrar empleado"} pending={pending} cancelHref="/employees" />
      )}
    </form>
  );
}
