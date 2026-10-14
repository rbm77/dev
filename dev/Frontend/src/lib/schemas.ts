import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { isoDate, optionalEmail, optionalPhone, optionalText, requiredText } from "@/lib/validation";

/** PUT /maintenances/{id}/complete and PUT /custom-transports/{id}/complete */
export const completeSchema = z.object({ completedDate: isoDate() });

export type CompleteFormValues = z.input<typeof completeSchema>;

/** Common/Shared/PersonalData.cs fields (personal_data table), used by users and employees. */
export const personalDataShape = {
  identityDocument: requiredText(LIMITS.personalData.identityDocument),
  name: requiredText(LIMITS.personalData.name),
  lastName: requiredText(LIMITS.personalData.lastName),
  address: optionalText(LIMITS.personalData.address),
  phoneNumber: optionalPhone(LIMITS.personalData.phoneNumber),
  email: optionalEmail(LIMITS.personalData.email),
};

export type PersonalDataFormValues = {
  identityDocument: string;
  name: string;
  lastName: string;
  address: string;
  phoneNumber: string;
  email: string;
};

/** Default form values from an API object (missing properties become ""). */
export function personalDataDefaults(source?: Partial<Record<keyof PersonalDataFormValues, string>>): PersonalDataFormValues {
  return {
    identityDocument: source?.identityDocument ?? "",
    name: source?.name ?? "",
    lastName: source?.lastName ?? "",
    address: source?.address ?? "",
    phoneNumber: source?.phoneNumber ?? "",
    email: source?.email ?? "",
  };
}
