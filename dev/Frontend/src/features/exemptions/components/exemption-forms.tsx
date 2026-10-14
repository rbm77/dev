"use client";

import { useRouter } from "next/navigation";
import { ComboboxField } from "@/components/forms/combobox-field";
import { DateField, PercentageField, SelectField, TextField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { createPeriodicExemption, createSpecificExemption, updatePeriodicExemption, updateSpecificExemption } from "../actions";
import {
  periodicExemptionSchema,
  specificExemptionSchema,
  type PeriodicExemptionFormValues,
  type SpecificExemptionFormValues,
} from "../schema";
import type { PeriodicExemption, SpecificExemption } from "../types";

type StudentPreset = { id: number; label: string };

const PERCENTAGE_LABEL = "Porcentaje de descuento (%)";
const PERCENTAGE_HELP = "100 % exonera el cobro completo.";

type SpecificProps = {
  exemption?: SpecificExemption;
  studentLabel?: string;
  presetStudent?: StudentPreset;
  periodOptions: Option[];
  readOnly?: boolean;
};

export function SpecificExemptionForm({ exemption, studentLabel, presetStudent, periodOptions, readOnly = false }: SpecificProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<SpecificExemptionFormValues>({
    schema: specificExemptionSchema,
    defaultValues: {
      studentId: exemption ? String(exemption.studentId) : presetStudent ? String(presetStudent.id) : "",
      paymentPeriodId: exemption ? String(exemption.paymentPeriodId) : "",
      description: exemption?.description ?? "",
      percentage: exemption ? String(exemption.percentage) : "",
    },
    submit: (values) => (exemption ? updateSpecificExemption(exemption.id, values) : createSpecificExemption(values)),
    successMessage: exemption ? "Exención actualizada." : "Exención registrada.",
    onSuccess: () => {
      if (!exemption) router.push("/exemptions/specific");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Exención específica" description="Descuento sobre una fecha de cobro puntual.">
          <ComboboxField
            control={control}
            name="studentId"
            label="Estudiante"
            required
            endpoint="/api/lookups/students"
            initialLabel={studentLabel ?? presetStudent?.label}
            placeholder="Buscar estudiante…"
            searchPlaceholder="Nombre, apellido o identificación"
            disabled={readOnly}
            className="md:col-span-2"
          />
          <SelectField control={control} name="paymentPeriodId" label="Fecha de cobro" required options={periodOptions} />
          <PercentageField control={control} name="percentage" label={PERCENTAGE_LABEL} required description={PERCENTAGE_HELP} />
          <TextField
            control={control}
            name="description"
            label="Motivo"
            required
            maxLength={LIMITS.specificExemption.description}
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={exemption ? "Guardar cambios" : "Registrar exención"} pending={pending} cancelHref="/exemptions/specific" />
      )}
    </form>
  );
}

type PeriodicProps = {
  exemption?: PeriodicExemption;
  studentLabel?: string;
  presetStudent?: StudentPreset;
  readOnly?: boolean;
};

export function PeriodicExemptionForm({ exemption, studentLabel, presetStudent, readOnly = false }: PeriodicProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<PeriodicExemptionFormValues>({
    schema: periodicExemptionSchema,
    defaultValues: {
      studentId: exemption ? String(exemption.studentId) : presetStudent ? String(presetStudent.id) : "",
      description: exemption?.description ?? "",
      startDate: exemption?.startDate ?? "",
      endDate: exemption?.endDate ?? "",
      percentage: exemption ? String(exemption.percentage) : "",
    },
    submit: (values) => (exemption ? updatePeriodicExemption(exemption.id, values) : createPeriodicExemption(values)),
    successMessage: exemption ? "Exención actualizada." : "Exención registrada.",
    onSuccess: () => {
      if (!exemption) router.push("/exemptions/periodic");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Exención periódica" description="Descuento sobre todas las fechas de cobro dentro de un rango.">
          <ComboboxField
            control={control}
            name="studentId"
            label="Estudiante"
            required
            endpoint="/api/lookups/students"
            initialLabel={studentLabel ?? presetStudent?.label}
            placeholder="Buscar estudiante…"
            searchPlaceholder="Nombre, apellido o identificación"
            disabled={readOnly}
            className="md:col-span-2"
          />
          <DateField control={control} name="startDate" label="Desde" required />
          <DateField control={control} name="endDate" label="Hasta" required />
          <PercentageField control={control} name="percentage" label={PERCENTAGE_LABEL} required description={PERCENTAGE_HELP} />
          <TextField control={control} name="description" label="Motivo" required maxLength={LIMITS.periodicExemption.description} />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={exemption ? "Guardar cambios" : "Registrar exención"} pending={pending} cancelHref="/exemptions/periodic" />
      )}
    </form>
  );
}
