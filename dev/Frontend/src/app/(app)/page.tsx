import { Suspense } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import { ChevronRight, Compass } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { EmptyState } from "@/components/shared/empty-state";
import { PageHeader } from "@/components/shared/page-header";
import { getPendingCounters, hasCounterPermissions } from "@/features/home/counters";
import { visibleNavGroups } from "@/lib/navigation";
import type { Permission } from "@/lib/permissions";
import { verifySession } from "@/lib/session";
import { cn } from "@/lib/utils";

export const metadata: Metadata = { title: "Inicio" };

export default async function HomePage() {
  const { username, permissions } = await verifySession();
  const groups = visibleNavGroups(permissions);

  return (
    <>
      <PageHeader title={`Hola, ${username}`} description="Esto es lo que requiere tu atención." />

      {hasCounterPermissions(permissions) && (
        <section aria-labelledby="pending-title" className="mb-8">
          <h2 id="pending-title" className="sr-only">
            Pendientes
          </h2>
          <Suspense fallback={<CountersSkeleton />}>
            <PendingCounters permissions={permissions} />
          </Suspense>
        </section>
      )}

      {groups.length === 0 ? (
        <EmptyState
          icon={Compass}
          title="Aún no tienes secciones asignadas"
          description="Tu rol no tiene permisos sobre ningún módulo. Comunícate con el administrador."
        />
      ) : (
        <section aria-labelledby="sections-title">
          <h2 id="sections-title" className="mb-3 text-sm font-semibold text-muted-foreground">
            Secciones
          </h2>
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {groups.map((group) => (
              <Card key={group.id} className="gap-3">
                <CardHeader className="flex flex-row items-center gap-3">
                  <span className="flex size-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
                    <group.icon className="size-5" aria-hidden />
                  </span>
                  <CardTitle className="text-base">{group.label}</CardTitle>
                </CardHeader>
                <CardContent className="px-3">
                  <ul className="flex flex-col">
                    {group.items.map((item) => (
                      <li key={item.href}>
                        <Link
                          href={item.href}
                          className="flex min-h-11 items-center justify-between gap-2 rounded-md px-3 text-sm hover:bg-accent hover:text-accent-foreground md:min-h-9"
                        >
                          <span className="flex items-center gap-2.5">
                            <item.icon className="size-4 text-muted-foreground" aria-hidden />
                            {item.label}
                          </span>
                          <ChevronRight className="size-4 text-muted-foreground" aria-hidden />
                        </Link>
                      </li>
                    ))}
                  </ul>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>
      )}
    </>
  );
}

async function PendingCounters({ permissions }: { permissions: Permission[] }) {
  const counters = await getPendingCounters(permissions);
  if (counters.length === 0) {
    return null;
  }

  return (
    <ul className="grid grid-cols-2 gap-3 lg:grid-cols-4">
      {counters.map((counter) => {
        const pending = counter.count > 0;
        return (
          <li key={counter.key}>
            <Link
              href={counter.href}
              className="flex h-full flex-col gap-2 rounded-xl border bg-card p-4 shadow-xs transition-colors hover:border-primary/30 hover:bg-accent/40"
            >
              <span className="flex items-center justify-between gap-2">
                <counter.icon className="size-5 text-muted-foreground" aria-hidden />
                <span
                  className={cn(
                    "text-2xl font-semibold tabular-nums",
                    pending && counter.tone === "danger" && "text-red-700",
                    pending && counter.tone === "warning" && "text-amber-700",
                  )}
                >
                  {counter.count.toLocaleString("es-CR")}
                </span>
              </span>
              <span className="text-sm font-medium leading-tight">{counter.label}</span>
              <span className="hidden text-xs text-muted-foreground sm:block">{counter.description}</span>
            </Link>
          </li>
        );
      })}
    </ul>
  );
}

function CountersSkeleton() {
  return (
    <div aria-busy="true" aria-label="Cargando pendientes" className="grid grid-cols-2 gap-3 lg:grid-cols-4">
      {Array.from({ length: 4 }, (_, index) => (
        <Skeleton key={index} className="h-28 rounded-xl" />
      ))}
    </div>
  );
}
