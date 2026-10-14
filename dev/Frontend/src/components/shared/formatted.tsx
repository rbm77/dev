import { formatDate } from "@/lib/dates";
import { formatMoney } from "@/lib/format";
import { cn } from "@/lib/utils";

/** Amount in colones with aligned digits. */
export function Money({ value, className }: { value: number | null | undefined; className?: string }) {
  return <span className={cn("tabular-nums whitespace-nowrap", className)}>{formatMoney(value)}</span>;
}

/** "yyyy-MM-dd" rendered as dd/mm/aaaa. */
export function DateText({ value, className }: { value: string | null | undefined; className?: string }) {
  if (!value) {
    return <span className={className}>—</span>;
  }
  return (
    <time dateTime={value} className={cn("tabular-nums whitespace-nowrap", className)}>
      {formatDate(value)}
    </time>
  );
}
