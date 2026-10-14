import Link from "next/link";
import { ShieldX } from "lucide-react";
import { Button } from "@/components/ui/button";
import { EmptyState } from "./empty-state";

/** Shown when the user opens a section their role cannot access. */
export function NoAccess() {
  return (
    <EmptyState
      icon={ShieldX}
      title="No tienes acceso a esta sección"
      description="Tu rol no tiene permiso para ver esta información. Si crees que es un error, comunícate con el administrador."
      action={
        <Button asChild variant="outline">
          <Link href="/">Ir al inicio</Link>
        </Button>
      }
    />
  );
}
