"use client";

import { useRouter } from "next/navigation";
import { DateField, SelectField, TextareaField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import type { Incident } from "@/features/catalogs/types";
import { LIMITS } from "@/lib/db-limits";
import { INCIDENT_TYPES } from "@/lib/enums";
import { createIncident, updateIncident } from "../actions";
import { incidentSchema, type IncidentFormValues } from "../schema";

type IncidentFormProps = {
  incident?: Incident;
  vehicleOptions: Option[];
  driverOptions: Option[];
  readOnly?: boolean;
};

// 0 means "not assigned" in the API (nullable columns), shown as an empty select.
const idValue = (id: number | undefined) => (id ? String(id) : "");

export function IncidentForm({ incident, vehicleOptions, driverOptions, readOnly = false }: IncidentFormProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<IncidentFormValues, unknown>({
    schema: incidentSchema,
    defaultValues: {
      date: incident?.date ?? "",
      type: incident ? String(incident.type) : "",
      vehicleId: idValue(incident?.vehicleId),
      driverId: idValue(incident?.driverId),
      description: incident?.description ?? "",
      correctiveActions: incident?.correctiveActions ?? "",
    },
    submit: (values) => (incident ? updateIncident(incident.id, values) : createIncident(values)),
    successMessage: incident ? "Incidente actualizado." : "Incidente registrado.",
    onSuccess: () => {
      if (!incident) router.push("/incidents");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Incidente">
          <DateField control={control} name="date" label="Fecha" required />
          <SelectField control={control} name="type" label="Tipo" required options={INCIDENT_TYPES} />
          <SelectField control={control} name="vehicleId" label="Vehículo" required options={vehicleOptions} />
          <SelectField control={control} name="driverId" label="Conductor" required options={driverOptions} />
          <TextareaField
            control={control}
            name="description"
            label="Descripción"
            maxLength={LIMITS.incident.description}
            placeholder="¿Qué pasó?"
          />
          <TextareaField
            control={control}
            name="correctiveActions"
            label="Acciones correctivas"
            maxLength={LIMITS.incident.correctiveActions}
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={incident ? "Guardar cambios" : "Registrar incidente"} pending={pending} cancelHref="/incidents" />
      )}
    </form>
  );
}
