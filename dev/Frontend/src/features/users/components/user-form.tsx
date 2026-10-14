"use client";

import { useRouter } from "next/navigation";
import { SelectField, SwitchField, TextField, type Option } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { PersonalDataFields } from "@/components/forms/personal-data-fields";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { personalDataDefaults } from "@/lib/schemas";
import { createUser, updateUser } from "../actions";
import { userSchema, type UserFormValues } from "../schema";
import type { User } from "../types";

type UserFormProps = {
  user?: User;
  roleOptions: Option[];
  readOnly?: boolean;
};

export function UserForm({ user, roleOptions, readOnly = false }: UserFormProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<UserFormValues, unknown>({
    schema: userSchema,
    defaultValues: {
      username: user?.username ?? "",
      roleId: user ? String(user.roleId) : "",
      isActive: user?.isActive ?? true,
      ...personalDataDefaults(user),
    },
    submit: (values) => (user ? updateUser(user.id, values) : createUser(values)),
    successMessage: user ? "Usuario actualizado." : "Usuario creado.",
    onSuccess: () => router.push("/settings/users"),
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="flex min-w-0 flex-col gap-6">
        <FormSection title="Acceso" description="Usuario con el que inicia sesión y rol asignado.">
          <TextField control={control} name="username" label="Usuario" required maxLength={LIMITS.user.username} autoComplete="off" />
          <SelectField control={control} name="roleId" label="Rol" required options={roleOptions} />
          <SwitchField
            control={control}
            name="isActive"
            label="Usuario activo"
            description="Los usuarios inactivos no pueden iniciar sesión."
            className="md:col-span-2"
          />
        </FormSection>
        <FormSection title="Datos personales">
          <PersonalDataFields control={control} />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={user ? "Guardar cambios" : "Crear usuario"} pending={pending} cancelHref="/settings/users" />
      )}
    </form>
  );
}
