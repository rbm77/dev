import type { FieldValues, Path, UseFormSetError } from "react-hook-form";

/** Shows server-side validation errors (from an ActionResult) on the form fields. */
export function applyFieldErrors<T extends FieldValues>(
  setError: UseFormSetError<T>,
  fieldErrors: Partial<Record<string, string[]>> | undefined,
): void {
  for (const [name, messages] of Object.entries(fieldErrors ?? {})) {
    const message = messages?.[0];
    if (message) {
      setError(name as Path<T>, { type: "server", message });
    }
  }
}
