import { renderAppIcon } from "@/lib/app-icon";

// Icons referenced by the web app manifest, generated once at build time.
const VARIANTS = {
  "192": { size: 192, glyphRatio: 0.62, rounded: true },
  "512": { size: 512, glyphRatio: 0.62, rounded: true },
  // Maskable icons are cropped to a circle by Android: keep the glyph inside
  // the 80% safe zone and fill the whole canvas.
  "maskable-512": { size: 512, glyphRatio: 0.46, rounded: false },
} as const;

type Variant = keyof typeof VARIANTS;

export const dynamicParams = false;

export function generateStaticParams() {
  return Object.keys(VARIANTS).map((variant) => ({ variant }));
}

export async function GET(_request: Request, { params }: RouteContext<"/pwa-icon/[variant]">) {
  const { variant } = await params;
  return renderAppIcon(VARIANTS[variant as Variant]);
}
