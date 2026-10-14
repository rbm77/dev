"use client";

import { TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { PersonalDataFields } from "@/components/forms/personal-data-fields";
import { SubmitButton } from "@/components/forms/submit-button";
import { useActionForm } from "@/components/forms/use-action-form";
import { LIMITS } from "@/lib/db-limits";
import { personalDataDefaults } from "@/lib/schemas";
import { updateProfile } from "../actions";
import { ownUserSchema, type OwnUserFormValues } from "../schema";
import type { User } from "../types";

export function ProfileForm({ user, readOnly }: { user: User; readOnly: boolean }) {
  const { control, pending, onSubmit } = useActionForm<OwnUserFormValues>({
    schema: ownUserSchema,
    defaultValues: { username: user.username ?? "", ...personalDataDefaults(user) },
    submit: updateProfile,
    successMessage: "Tus datos se actualizaron.",
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection
          title="Mis datos"
          description="Información de tu cuenta y de contacto."
          footer={!readOnly && <SubmitButton pending={pending} label="Guardar cambios" />}
        >
          <TextField
            control={control}
            name="username"
            label="Usuario"
            required
            maxLength={LIMITS.user.username}
            autoComplete="username"
            description="Lo usas para iniciar sesión."
          />
          <PersonalDataFields control={control} />
        </FormSection>
      </fieldset>
    </form>
  );
}
