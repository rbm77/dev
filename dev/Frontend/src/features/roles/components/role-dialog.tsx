"use client";

import { useState } from "react";
import Link from "next/link";
import { KeyRound, Pencil } from "lucide-react";
import { TextField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { CreateButton } from "@/components/shared/create-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { RowAction } from "@/components/shared/row-action";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import type { Role } from "@/features/catalogs/types";
import type { ActionResult } from "@/lib/action-result";
import { LIMITS } from "@/lib/db-limits";
import { createRole, updateRole } from "../actions";
import { roleSchema, type RoleFormValues } from "../schema";

function RoleFormDialog({ role, open, onOpenChange }: { role?: Role; open: boolean; onOpenChange: (open: boolean) => void }) {
  const { form, control, pending, onSubmit } = useActionForm<RoleFormValues>({
    schema: roleSchema,
    defaultValues: { description: role?.description ?? "" },
    submit: (values) => (role ? updateRole(role.id, values) : createRole(values)),
    successMessage: role ? "Rol actualizado." : "Rol creado.",
    onSuccess: () => {
      onOpenChange(false);
      if (!role) form.reset();
    },
  });

  return (
    <FormDialog
      open={open}
      onOpenChange={onOpenChange}
      title={role ? "Editar rol" : "Nuevo rol"}
      onSubmit={onSubmit}
      pending={pending}
    >
      <TextField control={control} name="description" label="Nombre del rol" required maxLength={LIMITS.role.description} />
    </FormDialog>
  );
}

export function CreateRoleButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nuevo rol" onClick={() => setOpen(true)} />
      <RoleFormDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

type RoleRowActionsProps = {
  role: Role;
  canWrite: boolean;
  deleteAction: () => Promise<ActionResult>;
};

export function RoleRowActions({ role, canWrite, deleteAction }: RoleRowActionsProps) {
  const [open, setOpen] = useState(false);
  const name = role.description ?? `#${role.id}`;

  return (
    <>
      <Tooltip>
        <TooltipTrigger asChild>
          <Button asChild variant="ghost" size="icon">
            <Link href={`/settings/roles/${role.id}`} aria-label={`Permisos de ${name}`}>
              <KeyRound aria-hidden />
            </Link>
          </Button>
        </TooltipTrigger>
        <TooltipContent>Permisos</TooltipContent>
      </Tooltip>
      {canWrite && (
        <>
          <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
          <DeleteButton action={deleteAction} entityLabel={`el rol ${name}`} compact />
          <RoleFormDialog role={role} open={open} onOpenChange={setOpen} />
        </>
      )}
    </>
  );
}
