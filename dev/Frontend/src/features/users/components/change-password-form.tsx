"use client";

import { PasswordField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { SubmitButton } from "@/components/forms/submit-button";
import { useActionForm } from "@/components/forms/use-action-form";
import { LIMITS } from "@/lib/db-limits";
import { changePassword } from "../actions";
import { changePasswordSchema, type ChangePasswordFormValues } from "../schema";

export function ChangePasswordForm() {
  const { form, control, pending, onSubmit } = useActionForm<ChangePasswordFormValues>({
    schema: changePasswordSchema,
    defaultValues: { password: "", confirmation: "" },
    submit: changePassword,
    successMessage: "Tu contraseña se actualizó.",
    onSuccess: () => form.reset(),
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={pending} className="min-w-0">
        <FormSection
          title="Cambiar contraseña"
          description={`Máximo ${LIMITS.user.password} caracteres.`}
          narrow
          footer={<SubmitButton pending={pending} label="Cambiar contraseña" />}
        >
          <PasswordField control={control} name="password" label="Nueva contraseña" required maxLength={LIMITS.user.password} />
          <PasswordField
            control={control}
            name="confirmation"
            label="Confirmar contraseña"
            required
            maxLength={LIMITS.user.password}
          />
        </FormSection>
      </fieldset>
    </form>
  );
}
