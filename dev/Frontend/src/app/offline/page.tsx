import type { Metadata } from "next";
import { WifiOff } from "lucide-react";
import { Brand } from "@/components/shared/brand";
import { EmptyState } from "@/components/shared/empty-state";
import { Button } from "@/components/ui/button";

export const metadata: Metadata = { title: "Sin conexión" };

// Cached by the service worker and shown when a page cannot be loaded offline.
// Works without JavaScript: "Reintentar" is a plain link that hits the network again.
export default function OfflinePage() {
  return (
    <main className="flex min-h-dvh flex-col items-center justify-center gap-8 px-4 py-10">
      <Brand />
      <EmptyState
        icon={WifiOff}
        title="Sin conexión"
        description="No pudimos conectarnos a internet. Revisa tu conexión y vuelve a intentarlo."
        className="w-full max-w-md"
        action={
          <Button asChild>
            {/* eslint-disable-next-line @next/next/no-html-link-for-pages -- full reload on purpose */}
            <a href="/">Reintentar</a>
          </Button>
        }
      />
    </main>
  );
}
