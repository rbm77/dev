"use client";

import { useState } from "react";
import { Pencil, Phone, Plus } from "lucide-react";
import { SwitchField, TextField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { DeleteButton } from "@/components/shared/delete-button";
import { EmptyState } from "@/components/shared/empty-state";
import { RowAction } from "@/components/shared/row-action";
import { ActiveBadge } from "@/components/shared/status-badge";
import { Button } from "@/components/ui/button";
import { LIMITS } from "@/lib/db-limits";
import { createContact, deleteContact, updateContact } from "../actions";
import { contactSchema, type ContactFormValues } from "../schema";
import type { Contact } from "../types";

type ContactDialogProps = {
  studentId: number;
  contact?: Contact;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

function ContactDialog({ studentId, contact, open, onOpenChange }: ContactDialogProps) {
  const { form, control, pending, onSubmit } = useActionForm<ContactFormValues>({
    schema: contactSchema,
    defaultValues: {
      phoneNumber: contact?.phoneNumber ?? "",
      description: contact?.description ?? "",
      isActive: contact?.isActive ?? true,
    },
    submit: (values) => (contact ? updateContact(studentId, contact.id, values) : createContact(studentId, values)),
    successMessage: contact ? "Contacto actualizado." : "Contacto agregado.",
    onSuccess: () => {
      onOpenChange(false);
      if (!contact) form.reset();
    },
  });

  return (
    <FormDialog open={open} onOpenChange={onOpenChange} title={contact ? "Editar contacto" : "Nuevo contacto"} onSubmit={onSubmit} pending={pending}>
      <TextField
        control={control}
        name="phoneNumber"
        label="Teléfono"
        type="tel"
        inputMode="tel"
        required
        maxLength={LIMITS.contact.phoneNumber}
      />
      <TextField
        control={control}
        name="description"
        label="Descripción"
        maxLength={LIMITS.contact.description}
        placeholder="Ej.: Madre, padre, encargado"
      />
      <SwitchField control={control} name="isActive" label="Contacto activo" />
    </FormDialog>
  );
}

function ContactActions({ studentId, contact }: { studentId: number; contact: Contact }) {
  const [open, setOpen] = useState(false);
  const label = contact.description ?? contact.phoneNumber ?? "este contacto";
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${label}`} onClick={() => setOpen(true)} />
      <DeleteButton action={() => deleteContact(studentId, contact.id)} entityLabel={`el contacto ${label}`} compact />
      <ContactDialog studentId={studentId} contact={contact} open={open} onOpenChange={setOpen} />
    </>
  );
}

export function ContactsPanel({ studentId, contacts, canWrite }: { studentId: number; contacts: Contact[]; canWrite: boolean }) {
  const [creating, setCreating] = useState(false);

  return (
    <div className="flex flex-col gap-4">
      {canWrite && (
        <div className="flex justify-end">
          <Button onClick={() => setCreating(true)}>
            <Plus aria-hidden />
            Agregar contacto
          </Button>
          <ContactDialog studentId={studentId} open={creating} onOpenChange={setCreating} />
        </div>
      )}

      {contacts.length === 0 ? (
        <EmptyState icon={Phone} title="Sin contactos" description="Agrega los teléfonos de los encargados del estudiante." />
      ) : (
        <ul className="grid gap-3 sm:grid-cols-2">
          {contacts.map((contact) => (
            <li key={contact.id} className="flex items-start justify-between gap-3 rounded-xl border bg-card p-4 shadow-xs">
              <div className="min-w-0">
                <p className="truncate font-medium">{contact.description ?? "Contacto"}</p>
                {contact.phoneNumber ? (
                  <a href={`tel:${contact.phoneNumber}`} className="text-sm text-primary underline-offset-4 hover:underline">
                    {contact.phoneNumber}
                  </a>
                ) : (
                  <p className="text-sm text-muted-foreground">Sin teléfono</p>
                )}
                <div className="mt-2">
                  <ActiveBadge active={contact.isActive} />
                </div>
              </div>
              {canWrite && (
                <div className="-mt-1.5 -mr-1.5 flex shrink-0 gap-1">
                  <ContactActions studentId={studentId} contact={contact} />
                </div>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
