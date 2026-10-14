"use client";

import { useState } from "react";
import { Pencil } from "lucide-react";
import { SwitchField, TextField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { CreateButton } from "@/components/shared/create-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { RowAction } from "@/components/shared/row-action";
import type { Route } from "@/features/catalogs/types";
import type { ActionResult } from "@/lib/action-result";
import { LIMITS } from "@/lib/db-limits";
import { createRoute, updateRoute } from "../actions";
import { routeSchema, type RouteFormValues } from "../schema";

function RouteFormDialog({ route, open, onOpenChange }: { route?: Route; open: boolean; onOpenChange: (open: boolean) => void }) {
  const { form, control, pending, onSubmit } = useActionForm<RouteFormValues>({
    schema: routeSchema,
    defaultValues: { name: route?.name ?? "", description: route?.description ?? "", isActive: route?.isActive ?? true },
    submit: (values) => (route ? updateRoute(route.id, values) : createRoute(values)),
    successMessage: route ? "Ruta actualizada." : "Ruta creada.",
    onSuccess: () => {
      onOpenChange(false);
      if (!route) form.reset();
    },
  });

  return (
    <FormDialog open={open} onOpenChange={onOpenChange} title={route ? "Editar ruta" : "Nueva ruta"} onSubmit={onSubmit} pending={pending}>
      <TextField control={control} name="name" label="Nombre" required maxLength={LIMITS.route.name} />
      <TextField control={control} name="description" label="Descripción" maxLength={LIMITS.route.description} />
      <SwitchField control={control} name="isActive" label="Ruta activa" />
    </FormDialog>
  );
}

export function CreateRouteButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nueva ruta" onClick={() => setOpen(true)} />
      <RouteFormDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

export function RouteRowActions({ route, deleteAction }: { route: Route; deleteAction: () => Promise<ActionResult> }) {
  const [open, setOpen] = useState(false);
  const name = route.name ?? `#${route.id}`;
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
      <DeleteButton action={deleteAction} entityLabel={`la ruta ${name}`} compact />
      <RouteFormDialog route={route} open={open} onOpenChange={setOpen} />
    </>
  );
}
