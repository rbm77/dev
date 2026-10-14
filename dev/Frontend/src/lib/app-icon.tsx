import { ImageResponse } from "next/og";
import { BRAND_COLOR } from "@/lib/brand";

type IconOptions = {
  size: number;
  /** Fraction of the canvas taken by the bus glyph. */
  glyphRatio: number;
  /** Rounded corners for "any" icons; maskable and Apple icons must be full-bleed. */
  rounded: boolean;
};

/** App icon: navy square with the Lucide "bus-front" glyph in white. */
export function renderAppIcon({ size, glyphRatio, rounded }: IconOptions): ImageResponse {
  const glyph = Math.round(size * glyphRatio);

  return new ImageResponse(
    (
      <div
        style={{
          width: "100%",
          height: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          background: BRAND_COLOR,
          borderRadius: rounded ? Math.round(size * 0.22) : 0,
        }}
      >
        <svg
          width={glyph}
          height={glyph}
          viewBox="0 0 24 24"
          fill="none"
          stroke="#FFFFFF"
          strokeWidth={2}
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M4 6 2 7" />
          <path d="M10 6h4" />
          <path d="m22 7-2-1" />
          <rect width="16" height="16" x="4" y="3" rx="2" />
          <path d="M4 11h16" />
          <path d="M8 15h.01" />
          <path d="M16 15h.01" />
          <path d="M6 19v2" />
          <path d="M18 21v-2" />
        </svg>
      </div>
    ),
    { width: size, height: size },
  );
}
