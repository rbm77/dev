import Link from "next/link";
import { ChevronRight } from "lucide-react";
import { Table, TableBody, TableCaption, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { cn } from "@/lib/utils";

export type Column<T> = {
  key: string;
  header: string;
  cell: (row: T) => React.ReactNode;
  /** Extra classes for the table cell and header (e.g. "text-right"). */
  className?: string;
  /** Title of the card on mobile and link target on desktop. Exactly one column should set it. */
  primary?: boolean;
  /** Hide the field in the mobile card (it still shows in the table). */
  hideOnMobile?: boolean;
};

type EntityListProps<T> = {
  rows: readonly T[];
  columns: readonly Column<T>[];
  rowKey: (row: T) => string | number;
  /** Detail page of the row. The primary cell (desktop) and the whole card (mobile) link to it. */
  rowHref?: (row: T) => string;
  /** Row actions (buttons, menus). Rendered above the card link so they stay clickable. */
  rowActions?: (row: T) => React.ReactNode;
  /** On phones, show the actions next to the title ("inline", icon buttons) or below the card ("footer", text buttons). */
  mobileActions?: "inline" | "footer";
  /** Accessible description of the table. */
  caption: string;
  /** Rendered instead of the list when there are no rows. */
  empty: React.ReactNode;
};

/** Responsive list: a table on md+ screens and tappable cards on phones. */
export function EntityList<T>({
  rows,
  columns,
  rowKey,
  rowHref,
  rowActions,
  mobileActions = "inline",
  caption,
  empty,
}: EntityListProps<T>) {
  if (rows.length === 0) {
    return <>{empty}</>;
  }

  const primary = columns.find((column) => column.primary) ?? columns[0];
  const details = columns.filter((column) => column !== primary && !column.hideOnMobile);

  return (
    <>
      <div className="hidden overflow-hidden rounded-xl border bg-card shadow-xs md:block">
        <Table>
          <TableCaption className="sr-only">{caption}</TableCaption>
          <TableHeader>
            <TableRow className="bg-muted/50 hover:bg-muted/50">
              {columns.map((column) => (
                <TableHead key={column.key} className={cn("h-11 px-4 text-muted-foreground", column.className)}>
                  {column.header}
                </TableHead>
              ))}
              {rowActions && (
                <TableHead className="h-11 w-0 px-4">
                  <span className="sr-only">Acciones</span>
                </TableHead>
              )}
            </TableRow>
          </TableHeader>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={rowKey(row)}>
                {columns.map((column) => (
                  <TableCell key={column.key} className={cn("px-4 py-3", column.className)}>
                    {column === primary && rowHref ? (
                      <Link href={rowHref(row)} className="font-medium text-primary underline-offset-4 hover:underline">
                        {column.cell(row)}
                      </Link>
                    ) : (
                      column.cell(row)
                    )}
                  </TableCell>
                ))}
                {rowActions && (
                  <TableCell className="px-4 py-2 text-right">
                    <div className="flex justify-end gap-1">{rowActions(row)}</div>
                  </TableCell>
                )}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <ul className="flex flex-col gap-2 md:hidden" aria-label={caption}>
        {rows.map((row) => (
          <li key={rowKey(row)} className="relative rounded-xl border bg-card p-4 shadow-xs">
            <div className="flex items-start justify-between gap-3">
              <div className="min-w-0 flex-1 font-medium">
                {rowHref ? (
                  // The ::after overlay makes the whole card tappable.
                  <Link href={rowHref(row)} className="after:absolute after:inset-0 after:rounded-xl">
                    {primary.cell(row)}
                  </Link>
                ) : (
                  primary.cell(row)
                )}
              </div>
              {rowActions && mobileActions === "inline" ? (
                <div className="relative z-10 -mt-1.5 -mr-1.5 flex shrink-0 gap-1">{rowActions(row)}</div>
              ) : (
                rowHref && <ChevronRight className="mt-0.5 size-4 shrink-0 text-muted-foreground" aria-hidden />
              )}
            </div>
            {details.length > 0 && (
              <dl className="mt-3 grid grid-cols-2 gap-x-4 gap-y-2 text-sm">
                {details.map((column) => (
                  <div key={column.key} className="min-w-0">
                    <dt className="text-xs text-muted-foreground">{column.header}</dt>
                    <dd className="truncate">{column.cell(row)}</dd>
                  </div>
                ))}
              </dl>
            )}
            {rowActions && mobileActions === "footer" && (
              <div className="relative z-10 mt-3 flex flex-wrap justify-end gap-2 border-t pt-3">{rowActions(row)}</div>
            )}
          </li>
        ))}
      </ul>
    </>
  );
}
