"use client";

import { useRouter } from "next/navigation";
import { DateField, IntegerField, MoneyField } from "@/components/forms/fields";
import { FormSection } from "@/components/forms/form-section";
import { useActionForm } from "@/components/forms/use-action-form";
import { FormActions } from "@/components/shared/form-actions";
import { createPaymentPeriodRequest, updatePaymentPeriodRequest } from "../actions";
import { paymentPeriodRequestSchema, type PaymentPeriodRequestFormValues } from "../schema";
import type { PaymentPeriodRequest } from "../types";

type Props = { request?: PaymentPeriodRequest; readOnly?: boolean };

export function PaymentPeriodRequestForm({ request, readOnly = false }: Props) {
  const router = useRouter();
  const { control, pending, onSubmit } = useActionForm<PaymentPeriodRequestFormValues, unknown>({
    schema: paymentPeriodRequestSchema,
    defaultValues: {
      amount: request ? String(request.amount) : "",
      startDate: request?.startDate ?? "",
      daysToNextPayment: request ? String(request.daysToNextPayment) : "30",
    },
    submit: (values) => (request ? updatePaymentPeriodRequest(request.id, values) : createPaymentPeriodRequest(values)),
    successMessage: request ? "Configuración actualizada." : "Configuración de cobro creada.",
    onSuccess: (id) => {
      if (!request) router.push(typeof id === "number" ? `/payment-periods/${id}` : "/payment-periods");
    },
  });

  return (
    <form onSubmit={onSubmit} noValidate>
      <fieldset disabled={readOnly || pending} className="min-w-0">
        <FormSection
          title="Configuración de cobro"
          description="A partir de la fecha de inicio, el sistema genera una fecha de cobro cada cierto número de días."
        >
          <MoneyField control={control} name="amount" label="Monto por periodo (₡)" required />
          <DateField control={control} name="startDate" label="Fecha de inicio" required />
          <IntegerField
            control={control}
            name="daysToNextPayment"
            label="Días entre cobros"
            required
            description="Por ejemplo: 30 para cobro mensual, 15 para quincenal."
          />
        </FormSection>
      </fieldset>
      {!readOnly && (
        <FormActions submitLabel={request ? "Guardar cambios" : "Crear configuración"} pending={pending} cancelHref="/payment-periods" />
      )}
    </form>
  );
}
