"use client";

import { useState } from "react";
import { Pencil } from "lucide-react";
import { TextField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { CreateButton } from "@/components/shared/create-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { RowAction } from "@/components/shared/row-action";
import type { Grade } from "@/features/catalogs/types";
import type { ActionResult } from "@/lib/action-result";
import { LIMITS } from "@/lib/db-limits";
import { createGrade, updateGrade } from "../actions";
import { gradeSchema, type GradeFormValues } from "../schema";

function GradeFormDialog({ grade, open, onOpenChange }: { grade?: Grade; open: boolean; onOpenChange: (open: boolean) => void }) {
  const { form, control, pending, onSubmit } = useActionForm<GradeFormValues>({
    schema: gradeSchema,
    defaultValues: { description: grade?.description ?? "" },
    submit: (values) => (grade ? updateGrade(grade.id, values) : createGrade(values)),
    successMessage: grade ? "Grado actualizado." : "Grado creado.",
    onSuccess: () => {
      onOpenChange(false);
      if (!grade) form.reset();
    },
  });

  return (
    <FormDialog open={open} onOpenChange={onOpenChange} title={grade ? "Editar grado" : "Nuevo grado"} onSubmit={onSubmit} pending={pending}>
      <TextField
        control={control}
        name="description"
        label="Nombre del grado"
        required
        maxLength={LIMITS.grade.description}
        placeholder="Ej.: Quinto grado"
      />
    </FormDialog>
  );
}

export function CreateGradeButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nuevo grado" onClick={() => setOpen(true)} />
      <GradeFormDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

export function GradeRowActions({ grade, deleteAction }: { grade: Grade; deleteAction: () => Promise<ActionResult> }) {
  const [open, setOpen] = useState(false);
  const name = grade.description ?? `#${grade.id}`;
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
      <DeleteButton action={deleteAction} entityLabel={`el grado ${name}`} compact />
      <GradeFormDialog grade={grade} open={open} onOpenChange={setOpen} />
    </>
  );
}
