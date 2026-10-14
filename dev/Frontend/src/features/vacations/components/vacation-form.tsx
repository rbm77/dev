"use client";

import { useRouter } from "next/navigation";
import { DateField, TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { createVacation, updateVacation } from "../actions";
import type { Vacation } from "../types";
import { vacationSchema, type VacationFormValues } from "../schema";

export function VacationForm({ vacation, readOnly = false }: { vacation?: Vacation; readOnly?: boolean }) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<VacationFormValues>({
    schema: vacationSchema,
    defaultValues: {
      description: vacation?.description ?? "",
      startDate: vacation?.startDate ?? "",
      endDate: vacation?.endDate ?? "",
    },
    submit: (values) => (vacation ? updateVacation(vacation.id, values) : createVacation(values)),
    successMessage: vacation ? "Vacaciones actualizadas." : "Vacaciones registradas.",
    onSuccess: () => {
      if (!vacation) router.push("/vacations");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection
          title="Periodo de vacaciones"
          description="Las fechas de cobro dentro de este periodo no se cobran a los estudiantes."
        >
          <TextField
            control={control}
            name="description"
            label="Descripción"
            required
            maxLength={LIMITS.vacation.description}
            placeholder="Ej.: Vacaciones de medio año"
            className="md:col-span-2"
          />
          <DateField control={control} name="startDate" label="Desde" required />
          <DateField control={control} name="endDate" label="Hasta" required />
        </FormSection>
      </fieldset>
      {!readOnly && <FormActions submitLabel={vacation ? "Guardar cambios" : "Registrar vacaciones"} pending={pending} cancelHref="/vacations" />}
    </form>
  );
}
