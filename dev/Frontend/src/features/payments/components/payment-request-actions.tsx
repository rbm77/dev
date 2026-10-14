"use client";

import { BadgeCheck, CheckCheck, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/shared/confirm-dialog";
import { formatMoney } from "@/lib/format";
import { approvePaymentRequest, rejectPaymentRequest, validatePaymentRequest } from "../actions";
import type { PaymentRequest } from "../types";

/** Validate → approve (creates the payment), or reject (deletes the request). */
export function PaymentRequestActions({ request }: { request: PaymentRequest }) {
  const summary = `${formatMoney(request.amount)} · referencia ${request.receiptReference}`;

  return (
    <>
      {request.isValidated ? (
        <ConfirmDialog
          trigger={
            <Button size="sm">
              <CheckCheck aria-hidden />
              Aprobar
            </Button>
          }
          title="¿Aprobar esta solicitud?"
          description={`Se registrará el pago (${summary}) y la solicitud dejará de aparecer en esta lista.`}
          confirmLabel="Aprobar y registrar pago"
          action={() => approvePaymentRequest(request.id)}
          successMessage="Pago registrado."
        />
      ) : (
        <ConfirmDialog
          trigger={
            <Button size="sm" variant="outline">
              <BadgeCheck aria-hidden />
              Validar
            </Button>
          }
          title="¿Validar esta solicitud?"
          description={`Confirma que el comprobante (${summary}) es correcto. Si la empresa tiene la aprobación automática activa, el pago se registrará en este mismo paso.`}
          confirmLabel="Validar"
          action={() => validatePaymentRequest(request.id)}
          successMessage="Solicitud validada."
        />
      )}
      <ConfirmDialog
        trigger={
          <Button size="sm" variant="ghost" className="text-destructive" aria-label={`Rechazar ${summary}`}>
            <X aria-hidden />
            <span className="sr-only sm:not-sr-only">Rechazar</span>
          </Button>
        }
        title="¿Rechazar esta solicitud?"
        description={`La solicitud (${summary}) se eliminará y no se registrará ningún pago.`}
        confirmLabel="Rechazar"
        destructive
        action={() => rejectPaymentRequest(request.id)}
        successMessage="Solicitud rechazada."
      />
    </>
  );
}
