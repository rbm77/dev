import Link from "next/link";
import { Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";

type FormActionsProps = {
  submitLabel: string;
  pending: boolean;
  /** Where "Cancelar" goes. Omit to hide the button. */
  cancelHref?: string;
};

/**
 * Save/cancel bar of full-page forms. On phones it sticks above the bottom
 * navigation so the main action is always reachable.
 */
export function FormActions({ submitLabel, pending, cancelHref }: FormActionsProps) {
  return (
    <div className="sticky bottom-[calc(4rem+env(safe-area-inset-bottom))] z-20 -mx-4 mt-6 flex flex-col-reverse gap-2 border-t bg-background/95 px-4 py-3 backdrop-blur sm:flex-row sm:justify-end lg:static lg:mx-0 lg:border-0 lg:bg-transparent lg:px-0 lg:py-0 lg:backdrop-blur-none">
      {cancelHref && (
        <Button asChild variant="outline" disabled={pending}>
          <Link href={cancelHref}>Cancelar</Link>
        </Button>
      )}
      <Button type="submit" disabled={pending}>
        {pending && <Loader2 className="animate-spin" aria-hidden />}
        {pending ? "Guardando…" : submitLabel}
      </Button>
    </div>
  );
}
