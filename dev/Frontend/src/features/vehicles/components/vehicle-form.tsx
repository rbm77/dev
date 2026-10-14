"use client";

import { useRouter } from "next/navigation";
import { DateField, IntegerField, SwitchField, TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import type { Vehicle } from "@/features/catalogs/types";
import { LIMITS } from "@/lib/db-limits";
import { createVehicle, updateVehicle } from "../actions";
import { vehicleSchema, type VehicleFormValues } from "../schema";

export function VehicleForm({ vehicle, readOnly = false }: { vehicle?: Vehicle; readOnly?: boolean }) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<VehicleFormValues, unknown>({
    schema: vehicleSchema,
    defaultValues: {
      licensePlate: vehicle?.licensePlate ?? "",
      make: vehicle?.make ?? "",
      model: vehicle?.model ?? "",
      manufactureYear: vehicle ? String(vehicle.manufactureYear) : "",
      capacity: vehicle ? String(vehicle.capacity) : "",
      mileage: vehicle ? String(vehicle.mileage) : "",
      acquisitionDate: vehicle?.acquisitionDate ?? "",
      isActive: vehicle?.isActive ?? true,
    },
    submit: (values) => (vehicle ? updateVehicle(vehicle.id, values) : createVehicle(values)),
    successMessage: vehicle ? "Vehículo actualizado." : "Vehículo registrado.",
    onSuccess: () => {
      if (!vehicle) router.push("/vehicles");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="flex min-w-0 flex-col gap-6">
        <FormSection title="Identificación">
          <TextField
            control={control}
            name="licensePlate"
            label="Placa"
            required
            maxLength={LIMITS.vehicle.licensePlate}
            className="md:col-span-2"
          />
          <TextField control={control} name="make" label="Marca" required maxLength={LIMITS.vehicle.make} />
          <TextField control={control} name="model" label="Modelo" required maxLength={LIMITS.vehicle.model} />
        </FormSection>
        <FormSection title="Características">
          <IntegerField control={control} name="manufactureYear" label="Año de fabricación" required />
          <IntegerField control={control} name="capacity" label="Capacidad (pasajeros)" required />
          <IntegerField control={control} name="mileage" label="Kilometraje" required />
          <DateField control={control} name="acquisitionDate" label="Fecha de adquisición" required />
          <SwitchField
            control={control}
            name="isActive"
            label="Vehículo activo"
            description="Los vehículos inactivos no se ofrecen en los formularios nuevos."
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={vehicle ? "Guardar cambios" : "Registrar vehículo"} pending={pending} cancelHref="/vehicles" />
      )}
    </form>
  );
}
