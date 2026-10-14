import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

export type StatusTone = "success" | "warning" | "danger" | "neutral" | "info";

// Text colors are one step darker than the base tokens to keep AA contrast.
const TONE_CLASSES: Record<StatusTone, string> = {
  success: "border-emerald-200 bg-emerald-50 text-emerald-700",
  warning: "border-amber-200 bg-amber-50 text-amber-700",
  danger: "border-red-200 bg-red-50 text-red-700",
  neutral: "border-slate-200 bg-slate-100 text-slate-600",
  info: "border-primary/20 bg-primary/10 text-primary",
};

export function StatusBadge({ tone, children }: { tone: StatusTone; children: React.ReactNode }) {
  return (
    <Badge variant="outline" className={cn("font-medium", TONE_CLASSES[tone])}>
      {children}
    </Badge>
  );
}

/** Common "Activo / Inactivo" badge used across catalogs ("Activa" for feminine nouns). */
export function ActiveBadge({ active, feminine = false }: { active: boolean; feminine?: boolean }) {
  const ending = feminine ? "a" : "o";
  return (
    <StatusBadge tone={active ? "success" : "neutral"}>{active ? `Activ${ending}` : `Inactiv${ending}`}</StatusBadge>
  );
}
