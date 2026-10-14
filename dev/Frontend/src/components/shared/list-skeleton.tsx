import { Skeleton } from "@/components/ui/skeleton";

/** Placeholder rows shown while list results load (filters stay usable). */
export function ResultsSkeleton({ rows = 6 }: { rows?: number }) {
  return (
    <div aria-busy="true" aria-label="Cargando resultados" className="flex flex-col gap-2">
      {Array.from({ length: rows }, (_, index) => (
        <Skeleton key={index} className="h-20 w-full rounded-xl md:h-12" />
      ))}
    </div>
  );
}

/** Placeholder shown while a whole page loads (header, filters and rows). */
export function ListSkeleton({ rows = 6 }: { rows?: number }) {
  return (
    <div aria-busy="true" aria-label="Cargando" className="flex flex-col gap-5">
      <div className="flex flex-col gap-2">
        <Skeleton className="h-7 w-48" />
        <Skeleton className="h-4 w-72 max-w-full" />
      </div>
      <Skeleton className="h-11 w-full md:h-9 md:w-72" />
      <ResultsSkeleton rows={rows} />
    </div>
  );
}
