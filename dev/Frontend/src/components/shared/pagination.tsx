import Link from "next/link";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { hrefWith, type SearchParams } from "@/lib/search-params";
import { cn } from "@/lib/utils";

type PaginationProps = {
  page: number;
  pageSize: number;
  totalCount: number;
  pathname: string;
  searchParams: SearchParams;
};

/** "Anterior · Página X de Y · Siguiente". Links keep the current filters. */
export function Pagination({ page, pageSize, totalCount, pathname, searchParams }: PaginationProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  if (totalCount === 0 || (totalPages === 1 && page === 1)) {
    return totalCount > 0 ? <ResultCount count={totalCount} className="mt-4" /> : null;
  }

  const previous = page > 1 ? hrefWith(pathname, searchParams, { page: page === 2 ? undefined : page - 1 }) : null;
  const next = page < totalPages ? hrefWith(pathname, searchParams, { page: page + 1 }) : null;

  return (
    <nav aria-label="Paginación" className="mt-4 flex flex-wrap items-center justify-between gap-3">
      <ResultCount count={totalCount} />
      <div className="flex w-full items-center justify-between gap-2 sm:w-auto">
        <PageLink href={previous} label="Anterior" direction="previous" />
        <span className="text-sm text-muted-foreground tabular-nums">
          Página {Math.min(page, totalPages)} de {totalPages}
        </span>
        <PageLink href={next} label="Siguiente" direction="next" />
      </div>
    </nav>
  );
}

function ResultCount({ count, className }: { count: number; className?: string }) {
  return (
    <p className={cn("text-sm text-muted-foreground", className)}>
      {count === 1 ? "1 resultado" : `${count.toLocaleString("es-CR")} resultados`}
    </p>
  );
}

function PageLink({ href, label, direction }: { href: string | null; label: string; direction: "previous" | "next" }) {
  const icon =
    direction === "previous" ? <ChevronLeft aria-hidden /> : <ChevronRight aria-hidden />;

  if (!href) {
    return (
      <Button variant="outline" disabled>
        {direction === "previous" && icon}
        {label}
        {direction === "next" && icon}
      </Button>
    );
  }

  return (
    <Button asChild variant="outline">
      <Link href={href} rel={direction === "previous" ? "prev" : "next"} scroll={false}>
        {direction === "previous" && icon}
        {label}
        {direction === "next" && icon}
      </Link>
    </Button>
  );
}
