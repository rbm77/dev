import Link from "next/link";
import { X } from "lucide-react";

type ActiveFilterProps = {
  label: string;
  value: string;
  /** Same page without this filter. */
  clearHref: string;
};

/** Pill for a filter that comes from another page (e.g. "Estudiante: Ana Mora"). */
export function ActiveFilter({ label, value, clearHref }: ActiveFilterProps) {
  return (
    <div className="mb-4 inline-flex max-w-full items-center gap-1 rounded-full border bg-card py-1 pr-1 pl-3 text-sm shadow-xs">
      <span className="text-muted-foreground">{label}:</span>
      <span className="truncate font-medium">{value}</span>
      <Link
        href={clearHref}
        aria-label={`Quitar filtro ${label}`}
        className="ml-1 flex size-8 shrink-0 items-center justify-center rounded-full text-muted-foreground hover:bg-muted hover:text-foreground"
      >
        <X className="size-4" aria-hidden />
      </Link>
    </div>
  );
}
