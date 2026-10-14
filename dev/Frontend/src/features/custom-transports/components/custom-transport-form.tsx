"use client";

import { useRouter } from "next/navigation";
import { DateField, MoneyField, SelectField, TextareaField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { createCustomTransport, updateCompletedCustomTransport, updateCustomTransport } from "../actions";
import { completedCustomTransportSchema, customTransportSchema, type CustomTransportFormValues } from "../schema";
import type { CustomTransport } from "../types";

type CustomTransportFormProps = {
  transport?: CustomTransport;
  vehicleOptions: Option[];
  driverOptions: Option[];
  readOnly?: boolean;
};

export function CustomTransportForm({ transport, vehicleOptions, driverOptions, readOnly = false }: CustomTransportFormProps) {
  const router = useRouter();
  // Once completed the backend no longer keeps the scheduled date.
  const completed = Boolean(transport?.completedDate);
  const { control, pending, onSubmit } = useActionForm<CustomTransportFormValues, unknown>({
    schema: completed ? completedCustomTransportSchema : customTransportSchema,
    defaultValues: {
      scheduledDate: transport?.scheduledDate ?? "",
      vehicleId: transport ? String(transport.vehicleId) : "",
      driverId: transport ? String(transport.driverId) : "",
      amount: transport ? String(transport.amount) : "",
      description: transport?.description ?? "",
    },
    submit: (values) => {
      if (!transport) return createCustomTransport(values);
      return completed ? updateCompletedCustomTransport(transport.id, values) : updateCustomTransport(transport.id, values);
    },
    successMessage: transport ? "Transporte actualizado." : "Transporte programado.",
    onSuccess: () => {
      if (!transport) router.push("/custom-transports");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Transporte especial" description="Servicios fuera de las rutas regulares.">
          {!completed && <DateField control={control} name="scheduledDate" label="Fecha programada" required />}
          <MoneyField control={control} name="amount" label="Monto (₡)" required />
          <SelectField control={control} name="vehicleId" label="Vehículo" required options={vehicleOptions} />
          <SelectField control={control} name="driverId" label="Conductor" required options={driverOptions} />
          <TextareaField
            control={control}
            name="description"
            label="Descripción"
            maxLength={LIMITS.customTransport.description}
            placeholder="Ej.: Excursión al museo"
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions
          submitLabel={transport ? "Guardar cambios" : "Programar transporte"}
          pending={pending}
          cancelHref="/custom-transports"
        />
      )}
    </form>
  );
}
