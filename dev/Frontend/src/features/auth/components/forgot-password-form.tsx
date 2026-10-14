"use client";

import { useState, useTransition } from "react";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertCircle, CheckCircle2, Loader2 } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { LIMITS } from "@/lib/db-limits";
import { applyFieldErrors } from "@/lib/forms";
import { resetPassword } from "../actions";
import { resetPasswordSchema, type ResetPasswordInput } from "../schema";

export function ForgotPasswordForm() {
  const [isPending, startTransition] = useTransition();
  const [formError, setFormError] = useState<string | null>(null);
  const [sent, setSent] = useState(false);
  const form = useForm<ResetPasswordInput>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: { username: "" },
  });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit((values) => {
    setFormError(null);
    startTransition(async () => {
      const result = await resetPassword(values);
      if (result.ok) {
        setSent(true);
      } else {
        setFormError(result.error);
        applyFieldErrors(form.setError, result.fieldErrors);
      }
    });
  });

  if (sent) {
    return (
      <div className="flex flex-col gap-5">
        <Alert>
          <CheckCircle2 aria-hidden className="text-success" />
          <AlertTitle>Solicitud recibida</AlertTitle>
          <AlertDescription>
            Si el usuario existe, se generó una contraseña temporal. Comunícate con el administrador de tu
            empresa para recibirla.
          </AlertDescription>
        </Alert>
        <Button asChild size="lg" className="w-full">
          <Link href="/login">Volver a iniciar sesión</Link>
        </Button>
      </div>
    );
  }

  return (
    <form onSubmit={onSubmit} noValidate>
      <FieldGroup className="gap-5">
        {formError && (
          <Alert variant="destructive">
            <AlertCircle aria-hidden />
            <AlertDescription>{formError}</AlertDescription>
          </Alert>
        )}

        <Field data-invalid={!!errors.username}>
          <FieldLabel htmlFor="username">Usuario</FieldLabel>
          <Input
            id="username"
            autoComplete="username"
            autoCapitalize="none"
            autoCorrect="off"
            spellCheck={false}
            autoFocus
            maxLength={LIMITS.user.username}
            aria-invalid={!!errors.username}
            {...form.register("username")}
          />
          <FieldDescription>Ingresa el usuario con el que inicias sesión.</FieldDescription>
          <FieldError errors={[errors.username]} />
        </Field>

        <div className="flex flex-col gap-2">
          <Button type="submit" size="lg" className="w-full" disabled={isPending}>
            {isPending && <Loader2 className="animate-spin" aria-hidden />}
            {isPending ? "Enviando…" : "Restablecer contraseña"}
          </Button>
          <Button asChild variant="ghost" size="lg" className="w-full">
            <Link href="/login">Volver a iniciar sesión</Link>
          </Button>
        </div>
      </FieldGroup>
    </form>
  );
}
