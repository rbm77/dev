"use client";

import { useTransition } from "react";
import { useForm, type DefaultValues, type FieldValues } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import type * as z from "zod";
import type { ActionResult } from "@/lib/action-result";
import { applyFieldErrors } from "@/lib/forms";

type UseActionFormOptions<TInput extends FieldValues, TData> = {
  schema: z.ZodType<unknown, TInput>;
  defaultValues: DefaultValues<TInput>;
  /** Server Action. Receives the raw form values; it validates them again with the same schema. */
  submit: (values: TInput) => Promise<ActionResult<TData>>;
  successMessage?: string;
  onSuccess?: (data: TData) => void;
};

/** react-hook-form + zod + Server Action with toasts and server-side field errors. */
export function useActionForm<TInput extends FieldValues, TData = void>({
  schema,
  defaultValues,
  submit,
  successMessage,
  onSuccess,
}: UseActionFormOptions<TInput, TData>) {
  const [pending, startTransition] = useTransition();
  // The transformed (output) values are not used client-side, hence `unknown`.
  const form = useForm<TInput, unknown, unknown>({
    resolver: zodResolver(schema),
    defaultValues,
  });

  // The client-side check runs first; the raw values (not the transformed ones)
  // are sent so the server can parse them with the very same schema.
  const onSubmit = form.handleSubmit(() => {
    const values = form.getValues();
    startTransition(async () => {
      const result = await submit(values);
      if (!result.ok) {
        toast.error(result.error);
        applyFieldErrors(form.setError, result.fieldErrors);
        return;
      }
      if (successMessage) {
        toast.success(successMessage);
      }
      onSuccess?.(result.data);
    });
  });

  return { form, control: form.control, pending, onSubmit };
}
