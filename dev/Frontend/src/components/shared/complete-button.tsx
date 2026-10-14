"use client";

import { useState } from "react";
import { CheckCircle2 } from "lucide-react";
import { DateField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { Button } from "@/components/ui/button";
import type { ActionResult } from "@/lib/action-result";
import { todayIso } from "@/lib/dates";
import { completeSchema, type CompleteFormValues } from "@/lib/schemas";

type CompleteButtonProps = {
  /** Server Action bound to the record id; receives { completedDate }. */
  action: (input: unknown) => Promise<ActionResult>;
  title: string;
  description: string;
};

/** "Marcar como completado" with the completion date (today by default). */
export function CompleteButton({ action, title, description }: CompleteButtonProps) {
  const [open, setOpen] = useState(false);
  const { control, pending, onSubmit } = useActionForm<CompleteFormValues>({
    schema: completeSchema,
    defaultValues: { completedDate: todayIso() },
    submit: action,
    successMessage: "Marcado como completado.",
    onSuccess: () => setOpen(false),
  });

  return (
    <>
      <Button onClick={() => setOpen(true)}>
        <CheckCircle2 aria-hidden />
        Marcar como completado
      </Button>
      <FormDialog
        open={open}
        onOpenChange={setOpen}
        title={title}
        description={description}
        onSubmit={onSubmit}
        pending={pending}
        submitLabel="Completar"
      >
        <DateField control={control} name="completedDate" label="Fecha de realización" required />
      </FormDialog>
    </>
  );
}
