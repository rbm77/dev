import type { Metadata } from "next";
import Link from "next/link";
import { SearchX } from "lucide-react";
import { Brand } from "@/components/shared/brand";
import { EmptyState } from "@/components/shared/empty-state";
import { Button } from "@/components/ui/button";

export const metadata: Metadata = { title: "Página no encontrada" };

export default function NotFound() {
  return (
    <main className="flex min-h-dvh flex-col items-center justify-center gap-8 px-4 py-10">
      <Brand />
      <EmptyState
        icon={SearchX}
        title="Página no encontrada"
        description="La dirección no existe o la sección todavía no está disponible."
        className="w-full max-w-md"
        action={
          <Button asChild>
            <Link href="/">Ir al inicio</Link>
          </Button>
        }
      />
    </main>
  );
}
