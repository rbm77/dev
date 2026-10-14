"use client";

import Link from "next/link";
import { RotateCw, TriangleAlert } from "lucide-react";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/shared/empty-state";

// Production errors from Server Components arrive with a generic message, so a
// fixed, friendly text is shown instead of error.message.
export default function AppError({ retry }: { error: Error & { digest?: string }; retry: () => void }) {
  return (
    <EmptyState
      icon={TriangleAlert}
      title="No se pudo cargar esta sección"
      description="Ocurrió un problema al comunicarse con el servidor. Intenta de nuevo en unos momentos."
      action={
        <div className="flex flex-col gap-2 sm:flex-row">
          <Button onClick={() => retry()}>
            <RotateCw aria-hidden />
            Reintentar
          </Button>
          <Button asChild variant="outline">
            <Link href="/">Ir al inicio</Link>
          </Button>
        </div>
      }
    />
  );
}
