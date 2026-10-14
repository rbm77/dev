"use client";

import { useRouter } from "next/navigation";
import { ComboboxField } from "@/components/forms/combobox-field";
import { DateField, MoneyField, TextField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { LIMITS } from "@/lib/db-limits";
import { createPayment, updatePayment } from "../actions";
import { paymentSchema, type PaymentFormValues } from "../schema";
import type { Payment } from "../types";

type PaymentFormProps = {
  payment?: Payment;
  studentLabel?: string;
  /** Student preselected from another page (?studentId=). */
  presetStudent?: { id: number; label: string };
  readOnly?: boolean;
};

export function PaymentForm({ payment, studentLabel, presetStudent, readOnly = false }: PaymentFormProps) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<PaymentFormValues>({
    schema: paymentSchema,
    defaultValues: {
      studentId: payment ? String(payment.studentId) : presetStudent ? String(presetStudent.id) : "",
      date: payment?.date ?? "",
      amount: payment ? String(payment.amount) : "",
      receiptReference: payment?.receiptReference ?? "",
    },
    submit: (values) => (payment ? updatePayment(payment.id, values) : createPayment(values)),
    successMessage: payment ? "Pago actualizado." : "Pago registrado.",
    onSuccess: () => {
      if (!payment) router.push("/payments");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection title="Pago">
          <ComboboxField
            control={control}
            name="studentId"
            label="Estudiante"
            required
            endpoint="/api/lookups/students"
            initialLabel={studentLabel ?? presetStudent?.label}
            placeholder="Buscar estudiante…"
            searchPlaceholder="Nombre, apellido o identificación"
            disabled={readOnly}
            className="md:col-span-2"
          />
          <DateField control={control} name="date" label="Fecha del pago" required />
          <MoneyField control={control} name="amount" label="Monto (₡)" required />
          <TextField
            control={control}
            name="receiptReference"
            label="Referencia del comprobante"
            required
            maxLength={LIMITS.payment.receiptReference}
            description="Número de comprobante o transferencia. Debe ser único."
            className="md:col-span-2"
          />
        </FormSection>
      </fieldset>
      {!readOnly && <FormActions submitLabel={payment ? "Guardar cambios" : "Registrar pago"} pending={pending} cancelHref="/payments" />}
    </form>
  );
}
