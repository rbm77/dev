import * as z from "zod";
import { isIsoDate } from "@/lib/dates";
import { NUMBER_LIMITS } from "@/lib/db-limits";

// Shared zod building blocks. The same schemas validate on the client (form) and
// again on the server (Server Action) before anything reaches the API.

// The CSP forbids eval. Without this, zod probes it with `new Function` and the
// browser reports a CSP violation. Every schema module imports this file first.
z.config({ jitless: true });

export const MESSAGES = {
  required: "Este campo es obligatorio.",
  maxChars: (max: number) => `Máximo ${max} caracteres.`,
  email: "Ingresa un correo válido.",
  phone: "Solo se permiten números, espacios, + y -.",
  date: "Ingresa una fecha válida.",
  number: "Ingresa un número válido.",
  select: "Selecciona una opción.",
} as const;

const emptyToNull = (value: string) => (value === "" ? null : value);
const emptyToUndefined = (value: unknown) => (value === "" || value === null ? undefined : value);

/** Required text (NOT NULL column). */
export function requiredText(max: number) {
  return z.string({ error: MESSAGES.required }).trim().min(1, MESSAGES.required).max(max, MESSAGES.maxChars(max));
}

/** Optional text (nullable column). Empty input is sent as null. */
export function optionalText(max: number) {
  return z.string().trim().max(max, MESSAGES.maxChars(max)).transform(emptyToNull);
}

export function requiredEmail(max: number) {
  return requiredText(max).pipe(z.email(MESSAGES.email));
}

export function optionalEmail(max: number) {
  return z
    .string()
    .trim()
    .max(max, MESSAGES.maxChars(max))
    .refine((value) => value === "" || z.email().safeParse(value).success, MESSAGES.email)
    .transform(emptyToNull);
}

export function optionalPhone(max: number) {
  return z
    .string()
    .trim()
    .max(max, MESSAGES.maxChars(max))
    .regex(/^[0-9+\-\s]*$/, MESSAGES.phone)
    .transform(emptyToNull);
}

/** Required "yyyy-MM-dd" date. */
export function isoDate() {
  return z.string({ error: MESSAGES.required }).min(1, MESSAGES.required).refine(isIsoDate, MESSAGES.date);
}

/** Optional "yyyy-MM-dd" date. Empty input is sent as null. */
export function optionalIsoDate() {
  return z
    .string()
    .refine((value) => value === "" || isIsoDate(value), MESSAGES.date)
    .transform(emptyToNull);
}

function requiredNumber() {
  return z.coerce.number({
    error: (issue) => (issue.input === undefined ? MESSAGES.required : MESSAGES.number),
  });
}

const hasAtMostTwoDecimals = (value: number) => Math.abs(value * 100 - Math.round(value * 100)) < 1e-6;

/** DECIMAL(10,2) amount greater than zero. */
export function money() {
  return z.preprocess(
    emptyToUndefined,
    requiredNumber()
      .positive("El monto debe ser mayor a 0.")
      .max(NUMBER_LIMITS.money.max, "El monto es demasiado grande.")
      .refine(hasAtMostTwoDecimals, "Máximo 2 decimales."),
  );
}

/** DECIMAL(10,2) percentage between 0.01 and 100. */
export function percentage() {
  return z.preprocess(
    emptyToUndefined,
    requiredNumber()
      .min(NUMBER_LIMITS.percentage.min, "El porcentaje debe ser mayor a 0.")
      .max(NUMBER_LIMITS.percentage.max, "El porcentaje no puede ser mayor a 100.")
      .refine(hasAtMostTwoDecimals, "Máximo 2 decimales."),
  );
}

/** Whole number within a range (MySQL INT columns). */
export function integer(min: number, max: number = NUMBER_LIMITS.int.max) {
  return z.preprocess(
    emptyToUndefined,
    requiredNumber()
      .int("Ingresa un número entero.")
      .min(min, `El valor mínimo es ${min}.`)
      .max(max, `El valor máximo es ${max}.`),
  );
}

/** Reference to another entity chosen in a select (routeId, vehicleId, ...). */
export function entityId() {
  return z.preprocess(
    emptyToUndefined,
    z.coerce.number({ error: MESSAGES.select }).int(MESSAGES.select).positive(MESSAGES.select).max(NUMBER_LIMITS.int.max),
  );
}

/** Numeric enum chosen in a select (MaintenanceType 1–3, IncidentType 1–5). */
export function enumValue(max: number) {
  return z.preprocess(
    emptyToUndefined,
    z.coerce.number({ error: MESSAGES.select }).int(MESSAGES.select).min(1, MESSAGES.select).max(max, MESSAGES.select),
  );
}

const idSchema = z.coerce.number().int().positive().max(Number.MAX_SAFE_INTEGER);

/** Parses an id coming from the URL or the client. Returns null when it is not a positive integer. */
export function parseId(value: unknown): number | null {
  const result = idSchema.safeParse(value);
  return result.success ? result.data : null;
}
