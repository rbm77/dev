"use client";

import { useRouter } from "next/navigation";
import { DateField, SelectField, TextareaField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import type { Maintenance } from "@/features/catalogs/types";
import { LIMITS } from "@/lib/db-limits";
import { MAINTENANCE_TYPES } from "@/lib/enums";
import { createMaintenance, updateCompletedMaintenance, updateMaintenance } from "../actions";
import { completedMaintenanceSchema, maintenanceSchema, type MaintenanceFormValues } from "../schema";

type MaintenanceFormProps = { maintenance?: Maintenance; vehicleOptions: Option[]; readOnly?: boolean };

export function MaintenanceForm({ maintenance, vehicleOptions, readOnly = false }: MaintenanceFormProps) {
  const router = useRouter();
  // Once completed the backend no longer keeps the scheduled date.
  const completed = Boolean(maintenance?.completedDate);
  const { control, pending, onSubmit } = useActionForm<MaintenanceFormValues, unknown>({
    schema: completed ? completedMaintenanceSchema : maintenanceSchema,
    defaultValues: {
      vehicleId: maintenance ? String(maintenance.vehicleId) : "",
      type: maintenance ? String(maintenance.type) : "",
      scheduledDate: maintenance?.scheduledDate ?? "",
      description: maintenance?.description ?? "",
    },
    submit: (values) => {
      if (!maintenance) return createMaintenance(values);
      return completed ? updateCompletedMaintenance(maintenance.id, values) : updateMaintenance(maintenance.id, values);
    },
    successMessage: maintenance ? "Mantenimiento actualizado." : "Mantenimiento programado.",
    onSuccess: () => {
      if (!maintenance) router.push("/maintenances");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Mantenimiento">
          <SelectField control={control} name="vehicleId" label="Vehículo" required options={vehicleOptions} />
          <SelectField control={control} name="type" label="Tipo" required options={MAINTENANCE_TYPES} />
          {!completed && <DateField control={control} name="scheduledDate" label="Fecha programada" required />}
          <TextareaField
            control={control}
            name="description"
            label="Descripción"
            maxLength={LIMITS.maintenance.description}
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={maintenance ? "Guardar cambios" : "Programar mantenimiento"} pending={pending} cancelHref="/maintenances" />
      )}
    </form>
  );
}
