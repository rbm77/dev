import { renderAppIcon } from "@/lib/app-icon";

export const size = { width: 180, height: 180 };
export const contentType = "image/png";

// iOS applies its own rounded mask, so the icon is a full square.
export default function AppleIcon() {
  return renderAppIcon({ size: size.width, glyphRatio: 0.58, rounded: false });
}
