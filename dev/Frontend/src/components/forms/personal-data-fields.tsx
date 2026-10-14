"use client";

import type { Control, FieldValues, Path } from "react-hook-form";
import { LIMITS } from "@/lib/db-limits";
import { TextField } from "./fields";

type PersonalDataFieldsProps<T extends FieldValues> = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- matches react-hook-form's own Control default
  control: Control<T, any, any>;
  disabled?: boolean;
};

/** Identity, names and contact fields shared by users, employees and the profile. */
export function PersonalDataFields<T extends FieldValues>({ control, disabled }: PersonalDataFieldsProps<T>) {
  const name = (field: string) => field as Path<T>;

  return (
    <>
      <TextField control={control} name={name("name")} label="Nombre" required maxLength={LIMITS.personalData.name} autoComplete="given-name" disabled={disabled} />
      <TextField control={control} name={name("lastName")} label="Apellidos" required maxLength={LIMITS.personalData.lastName} autoComplete="family-name" disabled={disabled} />
      <TextField control={control} name={name("identityDocument")} label="Identificación" required maxLength={LIMITS.personalData.identityDocument} disabled={disabled} />
      <TextField control={control} name={name("phoneNumber")} label="Teléfono" type="tel" inputMode="tel" maxLength={LIMITS.personalData.phoneNumber} autoComplete="tel" disabled={disabled} />
      <TextField control={control} name={name("email")} label="Correo electrónico" type="email" inputMode="email" maxLength={LIMITS.personalData.email} autoComplete="email" disabled={disabled} />
      <TextField control={control} name={name("address")} label="Dirección" maxLength={LIMITS.personalData.address} autoComplete="street-address" disabled={disabled} />
    </>
  );
}
