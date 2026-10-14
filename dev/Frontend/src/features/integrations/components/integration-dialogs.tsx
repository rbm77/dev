"use client";

import { useState } from "react";
import { Pencil } from "lucide-react";
import { IntegerField, PasswordField, SwitchField, TextField } from "@/components/forms/fields";
import { FormDialog } from "@/components/forms/form-dialog";
import { useActionForm } from "@/components/forms/use-action-form";
import { CreateButton } from "@/components/shared/create-button";
import { DeleteButton } from "@/components/shared/delete-button";
import { RowAction } from "@/components/shared/row-action";
import { LIMITS } from "@/lib/db-limits";
import {
  deleteEmailAccount,
  deleteEmailSender,
  deleteWhatsAppAccount,
  saveEmailAccount,
  saveEmailSender,
  saveWhatsAppAccount,
} from "../actions";
import {
  emailAccountSchema,
  emailSenderSchema,
  whatsAppAccountSchema,
  type EmailAccountFormValues,
  type EmailSenderFormValues,
  type WhatsAppAccountFormValues,
} from "../schema";
import type { EmailAccount, EmailSender, WhatsAppAccount } from "../types";

type DialogProps<T> = { item?: T; open: boolean; onOpenChange: (open: boolean) => void };

// ---------------------------------------------------------------- e-mail accounts

function EmailAccountDialog({ item, open, onOpenChange }: DialogProps<EmailAccount>) {
  const defaults: EmailAccountFormValues = {
    emailAddress: item?.emailAddress ?? "",
    appPassword: "",
    imapHost: item?.imapHost ?? "imap.gmail.com",
    imapPort: item ? String(item.imapPort) : "993",
    isActive: item?.isActive ?? true,
  };
  const { form, control, pending, onSubmit } = useActionForm<EmailAccountFormValues>({
    schema: emailAccountSchema,
    defaultValues: defaults,
    submit: (values) => saveEmailAccount(item?.id ?? null, values),
    successMessage: item ? "Cuenta de correo actualizada." : "Cuenta de correo agregada.",
    onSuccess: () => {
      onOpenChange(false);
      // Never keep the secret in the form after saving.
      form.reset(item ? { ...form.getValues(), appPassword: "" } : defaults);
    },
  });

  return (
    <FormDialog
      open={open}
      onOpenChange={onOpenChange}
      title={item ? "Editar cuenta de correo" : "Nueva cuenta de correo"}
      description="Buzón que el sistema revisa para leer las notificaciones de pago del banco."
      onSubmit={onSubmit}
      pending={pending}
    >
      <TextField
        control={control}
        name="emailAddress"
        label="Correo electrónico"
        type="email"
        inputMode="email"
        required
        maxLength={LIMITS.emailAccount.emailAddress}
        autoComplete="off"
      />
      <PasswordField
        control={control}
        name="appPassword"
        label="Contraseña de aplicación"
        required
        maxLength={LIMITS.emailAccount.appPassword}
        description={
          item
            ? "Por seguridad no se muestra la actual: vuelve a ingresarla para guardar los cambios."
            : "Genérala en la configuración de seguridad de la cuenta de correo."
        }
      />
      <div className="grid gap-5 sm:grid-cols-[1fr_8rem]">
        <TextField control={control} name="imapHost" label="Servidor IMAP" required maxLength={LIMITS.emailAccount.imapHost} />
        <IntegerField control={control} name="imapPort" label="Puerto" required />
      </div>
      <SwitchField control={control} name="isActive" label="Cuenta activa" description="Solo se revisan las cuentas activas." />
    </FormDialog>
  );
}

export function CreateEmailAccountButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nueva cuenta de correo" onClick={() => setOpen(true)} />
      <EmailAccountDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

export function EmailAccountRowActions({ item }: { item: EmailAccount }) {
  const [open, setOpen] = useState(false);
  const name = item.emailAddress ?? `#${item.id}`;
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
      <DeleteButton action={() => deleteEmailAccount(item.id)} entityLabel={`la cuenta ${name}`} compact />
      <EmailAccountDialog item={item} open={open} onOpenChange={setOpen} />
    </>
  );
}

// ---------------------------------------------------------------- e-mail senders

function EmailSenderDialog({ item, open, onOpenChange }: DialogProps<EmailSender>) {
  const defaults: EmailSenderFormValues = {
    senderAddress: item?.senderAddress ?? "",
    description: item?.description ?? "",
    isActive: item?.isActive ?? true,
  };
  const { form, control, pending, onSubmit } = useActionForm<EmailSenderFormValues>({
    schema: emailSenderSchema,
    defaultValues: defaults,
    submit: (values) => saveEmailSender(item?.id ?? null, values),
    successMessage: item ? "Remitente actualizado." : "Remitente agregado.",
    onSuccess: () => {
      onOpenChange(false);
      if (!item) form.reset(defaults);
    },
  });

  return (
    <FormDialog
      open={open}
      onOpenChange={onOpenChange}
      title={item ? "Editar remitente" : "Nuevo remitente"}
      description="Solo se procesan los correos que llegan desde remitentes registrados (por ejemplo, el banco)."
      onSubmit={onSubmit}
      pending={pending}
    >
      <TextField
        control={control}
        name="senderAddress"
        label="Correo del remitente"
        type="email"
        inputMode="email"
        required
        maxLength={LIMITS.emailSender.senderAddress}
        autoComplete="off"
      />
      <TextField
        control={control}
        name="description"
        label="Descripción"
        maxLength={LIMITS.emailSender.description}
        placeholder="Ej.: Notificaciones del banco"
      />
      <SwitchField control={control} name="isActive" label="Remitente activo" />
    </FormDialog>
  );
}

export function CreateEmailSenderButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nuevo remitente" onClick={() => setOpen(true)} />
      <EmailSenderDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

export function EmailSenderRowActions({ item }: { item: EmailSender }) {
  const [open, setOpen] = useState(false);
  const name = item.senderAddress ?? `#${item.id}`;
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
      <DeleteButton action={() => deleteEmailSender(item.id)} entityLabel={`el remitente ${name}`} compact />
      <EmailSenderDialog item={item} open={open} onOpenChange={setOpen} />
    </>
  );
}

// ---------------------------------------------------------------- WhatsApp accounts

function WhatsAppAccountDialog({ item, open, onOpenChange }: DialogProps<WhatsAppAccount>) {
  const defaults: WhatsAppAccountFormValues = {
    phoneNumberId: item?.phoneNumberId ?? "",
    accessToken: "",
    isActive: item?.isActive ?? true,
  };
  const { form, control, pending, onSubmit } = useActionForm<WhatsAppAccountFormValues>({
    schema: whatsAppAccountSchema,
    defaultValues: defaults,
    submit: (values) => saveWhatsAppAccount(item?.id ?? null, values),
    successMessage: item ? "Cuenta de WhatsApp actualizada." : "Cuenta de WhatsApp agregada.",
    onSuccess: () => {
      onOpenChange(false);
      // Never keep the secret in the form after saving.
      form.reset(item ? { ...form.getValues(), accessToken: "" } : defaults);
    },
  });

  return (
    <FormDialog
      open={open}
      onOpenChange={onOpenChange}
      title={item ? "Editar cuenta de WhatsApp" : "Nueva cuenta de WhatsApp"}
      description="Número de WhatsApp Business donde los encargados envían los comprobantes de pago."
      onSubmit={onSubmit}
      pending={pending}
    >
      <TextField
        control={control}
        name="phoneNumberId"
        label="Phone Number ID"
        inputMode="numeric"
        required
        maxLength={LIMITS.whatsappAccount.phoneNumberId}
        autoComplete="off"
        description="Identificador del número en Meta (WhatsApp Business Platform)."
      />
      <PasswordField
        control={control}
        name="accessToken"
        label="Token de acceso"
        required
        maxLength={LIMITS.whatsappAccount.accessToken}
        description={item ? "Por seguridad no se muestra el actual: vuelve a ingresarlo para guardar los cambios." : undefined}
      />
      <SwitchField control={control} name="isActive" label="Cuenta activa" />
    </FormDialog>
  );
}

export function CreateWhatsAppAccountButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <CreateButton label="Nueva cuenta de WhatsApp" onClick={() => setOpen(true)} />
      <WhatsAppAccountDialog open={open} onOpenChange={setOpen} />
    </>
  );
}

export function WhatsAppAccountRowActions({ item }: { item: WhatsAppAccount }) {
  const [open, setOpen] = useState(false);
  const name = item.phoneNumberId ?? `#${item.id}`;
  return (
    <>
      <RowAction icon={Pencil} label={`Editar ${name}`} onClick={() => setOpen(true)} />
      <DeleteButton action={() => deleteWhatsAppAccount(item.id)} entityLabel={`la cuenta ${name}`} compact />
      <WhatsAppAccountDialog item={item} open={open} onOpenChange={setOpen} />
    </>
  );
}
