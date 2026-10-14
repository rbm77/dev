"use client";

import { SwitchField, TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { SubmitButton } from "@/components/forms/submit-button";
import { useActionForm } from "@/components/forms/use-action-form";
import { LIMITS } from "@/lib/db-limits";
import { saveCompany } from "../actions";
import { companySchema, type CompanyFormValues } from "../schema";
import type { Company } from "../types";

export function CompanyForm({ company, readOnly }: { company: Company; readOnly: boolean }) {
  const { control, pending, onSubmit } = useActionForm<CompanyFormValues>({
    schema: companySchema,
    defaultValues: {
      name: company.name ?? "",
      phoneNumber: company.phoneNumber ?? "",
      email: company.email ?? "",
      autoApprovalEnabled: company.autoApprovalEnabled,
    },
    submit: saveCompany,
    successMessage: "Datos de la empresa actualizados.",
  });

  return (
    <form onSubmit={onSubmit} noValidate className="flex flex-col gap-6">
      <fieldset disabled={readOnly || pending} className="flex min-w-0 flex-col gap-6">
        <FormSection title="Datos generales" description="Información de contacto de la empresa.">
          <TextField control={control} name="name" label="Nombre" required maxLength={LIMITS.company.name} className="md:col-span-2" />
          <TextField
            control={control}
            name="phoneNumber"
            label="Teléfono"
            type="tel"
            inputMode="tel"
            maxLength={LIMITS.company.phoneNumber}
          />
          <TextField
            control={control}
            name="email"
            label="Correo electrónico"
            type="email"
            inputMode="email"
            maxLength={LIMITS.company.email}
          />
        </FormSection>

        <FormSection
          title="Cobros"
          narrow
          footer={!readOnly && <SubmitButton pending={pending} label="Guardar cambios" />}
        >
          <SwitchField
            control={control}
            name="autoApprovalEnabled"
            label="Aprobación automática de pagos"
            description="Cuando una solicitud de pago se valida, se registra el pago sin intervención manual."
          />
        </FormSection>
      </fieldset>
    </form>
  );
}
