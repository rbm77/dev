"use client";

import { useState, useTransition } from "react";
import { Loader2, ShieldCheck, TriangleAlert, UserMinus } from "lucide-react";
import { toast } from "sonner";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { EmptyState } from "@/components/shared/empty-state";
import { EntityCombobox } from "@/components/shared/entity-combobox";
import { fullName } from "@/lib/format";
import { saveCriticalProcessUsers } from "../actions";
import type { CriticalProcessUser } from "../types";

type Entry = { id: number; label: string };

const sameIds = (a: readonly Entry[], b: readonly Entry[]) =>
  a.length === b.length && a.every((entry) => b.some((other) => other.id === entry.id));

export function CriticalUsersEditor({ initial, readOnly }: { initial: CriticalProcessUser[]; readOnly: boolean }) {
  const [saved, setSaved] = useState<Entry[]>(() =>
    initial.map((user) => ({ id: user.id, label: fullName(user.name, user.lastName) })),
  );
  const [entries, setEntries] = useState<Entry[]>(saved);
  const [pending, startTransition] = useTransition();
  const dirty = !sameIds(entries, saved);

  const add = (id: number, label: string) =>
    setEntries((current) => (current.some((entry) => entry.id === id) ? current : [...current, { id, label }]));

  const save = () =>
    startTransition(async () => {
      const result = await saveCriticalProcessUsers(entries.map((entry) => entry.id));
      if (result.ok) {
        setSaved(entries);
        toast.success("Lista de usuarios de proceso crítico actualizada.");
      } else {
        toast.error(result.error);
      }
    });

  return (
    <div className="flex flex-col gap-4">
      <Alert>
        <ShieldCheck aria-hidden />
        <AlertDescription>
          Solo estos usuarios pueden modificar o eliminar registros sensibles (pagos, gastos, estudiantes, usuarios,
          permisos, entre otros), además de tener el permiso de escritura correspondiente.
        </AlertDescription>
      </Alert>

      <Card className="gap-4">
        <CardHeader>
          <CardTitle className="text-base">Usuarios autorizados ({entries.length})</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          {!readOnly && (
            <EntityCombobox
              endpoint="/api/lookups/users"
              label="Agregar usuario"
              value=""
              placeholder="Agregar usuario…"
              searchPlaceholder="Nombre, apellido o identificación"
              onSelect={(item) => item && add(Number(item.value), item.label)}
            />
          )}

          {entries.length === 0 ? (
            <EmptyState icon={ShieldCheck} title="No hay usuarios de proceso crítico" className="py-8" />
          ) : (
            <ul className="divide-y rounded-lg border">
              {entries.map((entry) => (
                <li key={entry.id} className="flex items-center justify-between gap-3 px-3 py-2">
                  <span className="truncate text-sm font-medium">{entry.label}</span>
                  {!readOnly && (
                    <Button
                      variant="ghost"
                      size="icon"
                      aria-label={`Quitar a ${entry.label}`}
                      onClick={() => setEntries((current) => current.filter((item) => item.id !== entry.id))}
                    >
                      <UserMinus aria-hidden />
                    </Button>
                  )}
                </li>
              ))}
            </ul>
          )}

          {!readOnly && entries.length === 0 && saved.length > 0 && (
            <Alert variant="destructive">
              <TriangleAlert aria-hidden />
              <AlertDescription>
                Si guardas la lista vacía, nadie podrá modificar ni eliminar registros sensibles.
              </AlertDescription>
            </Alert>
          )}
        </CardContent>
      </Card>

      {!readOnly && (
        <div className="flex flex-col-reverse gap-2 sm:flex-row sm:justify-end">
          <Button variant="outline" disabled={!dirty || pending} onClick={() => setEntries(saved)}>
            Descartar cambios
          </Button>
          <Button disabled={!dirty || pending} onClick={save}>
            {pending && <Loader2 className="animate-spin" aria-hidden />}
            {pending ? "Guardando…" : "Guardar lista"}
          </Button>
        </div>
      )}
    </div>
  );
}
