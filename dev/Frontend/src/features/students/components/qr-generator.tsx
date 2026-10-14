"use client";

import { useState, useTransition } from "react";
import { Loader2, Printer, QrCode as QrCodeIcon, X } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Field, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { EmptyState } from "@/components/shared/empty-state";
import { EntityCombobox } from "@/components/shared/entity-combobox";
import { NUMBER_LIMITS } from "@/lib/db-limits";
import { generateQrCodes } from "../actions";
import type { QrCode } from "../types";

const SIZES = [
  { value: "10", label: "Pequeño" },
  { value: "20", label: "Mediano" },
  { value: "30", label: "Grande" },
];
const MAX = NUMBER_LIMITS.qrCodes.maxItems;

type Entry = { id: number; label: string };

export function QrGenerator() {
  const [selected, setSelected] = useState<Entry[]>([]);
  const [size, setSize] = useState("20");
  const [codes, setCodes] = useState<QrCode[] | null>(null);
  const [pending, startTransition] = useTransition();

  const add = (id: number, label: string) =>
    setSelected((current) =>
      current.length >= MAX || current.some((entry) => entry.id === id) ? current : [...current, { id, label }],
    );

  const generate = () =>
    startTransition(async () => {
      const result = await generateQrCodes({ studentIds: selected.map((entry) => entry.id), size: Number(size) });
      if (result.ok) {
        setCodes(result.data);
      } else {
        toast.error(result.error);
      }
    });

  return (
    <div className="flex flex-col gap-6">
      <Card className="gap-4 print:hidden">
        <CardHeader>
          <CardTitle className="text-base">Estudiantes ({selected.length}/{MAX})</CardTitle>
          <CardDescription>Busca y agrega hasta {MAX} estudiantes por hoja.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <EntityCombobox
            endpoint="/api/lookups/students"
            label="Agregar estudiante"
            value=""
            placeholder={selected.length >= MAX ? "Límite alcanzado" : "Agregar estudiante…"}
            searchPlaceholder="Nombre, apellido o identificación"
            disabled={selected.length >= MAX}
            onSelect={(item) => item && add(Number(item.value), item.label)}
          />
          {selected.length > 0 && (
            <ul className="flex flex-wrap gap-2">
              {selected.map((entry) => (
                <li key={entry.id} className="inline-flex items-center gap-1 rounded-full border bg-muted/50 py-1 pr-1 pl-3 text-sm">
                  {entry.label}
                  <button
                    type="button"
                    aria-label={`Quitar a ${entry.label}`}
                    onClick={() => setSelected((current) => current.filter((item) => item.id !== entry.id))}
                    className="flex size-7 items-center justify-center rounded-full text-muted-foreground hover:bg-muted hover:text-foreground"
                  >
                    <X className="size-4" aria-hidden />
                  </button>
                </li>
              ))}
            </ul>
          )}
          <div className="flex flex-col gap-3 sm:flex-row sm:items-end">
            <Field className="sm:w-48">
              <FieldLabel htmlFor="qr-size">Tamaño</FieldLabel>
              <Select value={size} onValueChange={setSize}>
                <SelectTrigger id="qr-size" className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {SIZES.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Field>
            <Button onClick={generate} disabled={selected.length === 0 || pending}>
              {pending ? <Loader2 className="animate-spin" aria-hidden /> : <QrCodeIcon aria-hidden />}
              {pending ? "Generando…" : "Generar códigos"}
            </Button>
          </div>
        </CardContent>
      </Card>

      {codes !== null &&
        (codes.length === 0 ? (
          <EmptyState icon={QrCodeIcon} title="No se generaron códigos" className="print:hidden" />
        ) : (
          <section aria-label="Códigos QR generados" className="flex flex-col gap-4">
            <div className="flex justify-end print:hidden">
              <Button variant="outline" onClick={() => window.print()}>
                <Printer aria-hidden />
                Imprimir
              </Button>
            </div>
            <ul className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 print:grid-cols-3">
              {codes.map((code, index) => (
                <li
                  key={`${code.description}-${index}`}
                  className="flex break-inside-avoid flex-col items-center gap-2 rounded-xl border bg-card p-4 text-center"
                >
                  {/* eslint-disable-next-line @next/next/no-img-element -- generated data URL, nothing to optimize */}
                  <img
                    src={`data:image/png;base64,${code.qrCodeBase64}`}
                    alt={`Código QR de ${code.description ?? "estudiante"}`}
                    className="aspect-square w-full max-w-48"
                  />
                  <span className="text-sm font-medium">{code.description}</span>
                </li>
              ))}
            </ul>
          </section>
        ))}
    </div>
  );
}
