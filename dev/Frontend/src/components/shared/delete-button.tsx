"use client";

import { useRouter } from "next/navigation";
import { Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { ActionResult } from "@/lib/action-result";
import { ConfirmDialog } from "./confirm-dialog";

type DeleteButtonProps = {
  /** Server Action bound to the id of the record. */
  action: () => Promise<ActionResult<unknown>>;
  /** Text after "Eliminar", e.g. "el vehículo ABC123". */
  entityLabel: string;
  /** Page to open after deleting (detail pages). Without it the current page refreshes. */
  redirectTo?: string;
  /** Icon-only button for table rows. */
  compact?: boolean;
};

export function DeleteButton({ action, entityLabel, redirectTo, compact = false }: DeleteButtonProps) {
  const router = useRouter();

  return (
    <ConfirmDialog
      trigger={
        compact ? (
          <Button variant="ghost" size="icon" aria-label={`Eliminar ${entityLabel}`} className="text-destructive">
            <Trash2 aria-hidden />
          </Button>
        ) : (
          <Button variant="destructive">
            <Trash2 aria-hidden />
            Eliminar
          </Button>
        )
      }
      title={`¿Eliminar ${entityLabel}?`}
      description="Esta acción no se puede deshacer. Si el registro está en uso, el sistema no permitirá eliminarlo."
      confirmLabel="Eliminar"
      destructive
      action={action}
      successMessage="Registro eliminado."
      onSuccess={() => (redirectTo ? router.push(redirectTo) : router.refresh())}
    />
  );
}
