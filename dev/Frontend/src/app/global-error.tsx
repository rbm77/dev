"use client";

import "./globals.css";

// Replaces the root layout when it fails, so it brings its own html/body.
export default function GlobalError({ retry }: { error: Error & { digest?: string }; retry: () => void }) {
  return (
    <html lang="es">
      <body className="flex min-h-dvh items-center justify-center bg-background px-4 font-sans text-foreground">
        <title>Error · Buslogix</title>
        <div className="flex max-w-sm flex-col items-center gap-4 text-center">
          <h1 className="text-xl font-semibold">Algo salió mal</h1>
          <p className="text-sm text-muted-foreground">
            Ocurrió un error inesperado. Intenta de nuevo; si el problema continúa, vuelve más tarde.
          </p>
          <button
            type="button"
            onClick={() => retry()}
            className="h-11 rounded-md bg-primary px-4 text-sm font-medium text-primary-foreground"
          >
            Reintentar
          </button>
        </div>
      </body>
    </html>
  );
}
