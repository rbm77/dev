import type { MetadataRoute } from "next";
import { BACKGROUND_COLOR, BRAND_COLOR } from "@/lib/brand";

export default function manifest(): MetadataRoute.Manifest {
  return {
    id: "/",
    name: "Buslogix",
    short_name: "Buslogix",
    description: "Gestión de transporte escolar: estudiantes, flota, cobros y pagos.",
    lang: "es-CR",
    dir: "ltr",
    start_url: "/",
    scope: "/",
    display: "standalone",
    background_color: BACKGROUND_COLOR,
    theme_color: BRAND_COLOR,
    categories: ["business", "productivity"],
    icons: [
      { src: "/pwa-icon/192", sizes: "192x192", type: "image/png", purpose: "any" },
      { src: "/pwa-icon/512", sizes: "512x512", type: "image/png", purpose: "any" },
      { src: "/pwa-icon/maskable-512", sizes: "512x512", type: "image/png", purpose: "maskable" },
    ],
  };
}
