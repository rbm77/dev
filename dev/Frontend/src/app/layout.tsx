import type { Metadata, Viewport } from "next";
import { Inter } from "next/font/google";
import { connection } from "next/server";
import "./globals.css";
import { cn } from "@/lib/utils";
import { BRAND_COLOR } from "@/lib/brand";
import { Toaster } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { ServiceWorkerRegistration } from "@/components/pwa/service-worker-registration";

const inter = Inter({ subsets: ["latin"], variable: "--font-sans" });

export const metadata: Metadata = {
  title: {
    template: "%s · Buslogix",
    default: "Buslogix",
  },
  description: "Gestión de transporte escolar: estudiantes, flota, cobros y pagos.",
  applicationName: "Buslogix",
  appleWebApp: {
    capable: true,
    title: "Buslogix",
    statusBarStyle: "default",
  },
  formatDetection: { telephone: false },
};

export const viewport: Viewport = {
  themeColor: BRAND_COLOR,
  colorScheme: "light",
  width: "device-width",
  initialScale: 1,
  viewportFit: "cover",
};

export default async function RootLayout({ children }: LayoutProps<"/">) {
  // Render every route per request so the CSP nonce set in proxy.ts is applied
  // to all scripts (prerendered pages cannot carry a per-request nonce).
  await connection();

  return (
    <html lang="es" className={cn("h-full antialiased font-sans", inter.variable)}>
      <body className="min-h-full flex flex-col">
        <TooltipProvider>{children}</TooltipProvider>
        <Toaster position="top-center" richColors closeButton />
        <ServiceWorkerRegistration />
      </body>
    </html>
  );
}
