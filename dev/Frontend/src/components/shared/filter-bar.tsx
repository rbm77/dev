"use client";

import { useCallback, useEffect, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { Search, SlidersHorizontal, X } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Field, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { InputGroup, InputGroupAddon, InputGroupButton, InputGroupInput } from "@/components/ui/input-group";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Sheet, SheetContent, SheetDescription, SheetFooter, SheetHeader, SheetTitle } from "@/components/ui/sheet";

export type FilterOption = { value: string; label: string };

export type FilterField =
  | { type: "search"; name: string; label: string; placeholder?: string; maxLength: number }
  | { type: "text"; name: string; label: string; placeholder?: string; maxLength: number }
  | { type: "select"; name: string; label: string; options: readonly FilterOption[]; allLabel?: string }
  | { type: "date"; name: string; label: string };

// Radix Select does not accept "" as an item value.
const ALL = "__all__";
const DEBOUNCE_MS = 400;

/** Replaces query params in the URL (empty removes) and resets pagination. */
function useUpdateParams() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  return useCallback(
    (changes: Record<string, string>) => {
      const next = new URLSearchParams(searchParams.toString());
      for (const [key, raw] of Object.entries(changes)) {
        const value = raw.trim();
        if (value) {
          next.set(key, value);
        } else {
          next.delete(key);
        }
      }
      next.delete("page");
      const query = next.toString();
      router.replace(query ? `${pathname}?${query}` : pathname, { scroll: false });
    },
    [router, pathname, searchParams],
  );
}

/**
 * Local value mirrored from a query param. `markApplied` records the value this
 * component pushed, so only changes made elsewhere (clear, back button) reset it.
 */
function useParamValue(name: string) {
  const searchParams = useSearchParams();
  const urlValue = searchParams.get(name) ?? "";
  const [value, setValue] = useState(urlValue);
  const [applied, setApplied] = useState(urlValue);

  if (urlValue !== applied) {
    setApplied(urlValue);
    setValue(urlValue);
  }

  return { value, setValue, applied, markApplied: setApplied };
}

type FilterBarProps = { fields: readonly FilterField[] };

export function FilterBar({ fields }: FilterBarProps) {
  const searchParams = useSearchParams();
  const updateParams = useUpdateParams();
  const search = fields.find((field) => field.type === "search");
  const filters = fields.filter((field) => field.type !== "search");
  const activeFilters = filters.filter((field) => searchParams.get(field.name)).length;
  const anyActive = activeFilters > 0 || (search !== undefined && Boolean(searchParams.get(search.name)));

  const clearAll = () => updateParams(Object.fromEntries(fields.map((field) => [field.name, ""])));

  return (
    <div className="mb-4 flex flex-col gap-3 md:flex-row md:flex-wrap md:items-end">
      <div className="flex gap-2 md:contents">
        {search && <DebouncedTextFilter field={search} updateParams={updateParams} className="flex-1 md:w-72 md:flex-none" />}
        {filters.length > 0 && (
          <MobileFilters fields={filters} activeCount={activeFilters} updateParams={updateParams} />
        )}
      </div>

      {filters.map((field) => (
        <div key={field.name} className="hidden md:block">
          {field.type === "text" ? (
            <DebouncedTextFilter field={field} updateParams={updateParams} className="w-56" />
          ) : (
            <ImmediateFilter field={field} updateParams={updateParams} />
          )}
        </div>
      ))}

      {anyActive && (
        <Button variant="ghost" onClick={clearAll} className="hidden md:inline-flex">
          <X aria-hidden />
          Limpiar
        </Button>
      )}
    </div>
  );
}

type FieldControlProps<F extends FilterField> = {
  field: F;
  updateParams: (changes: Record<string, string>) => void;
  className?: string;
};

function DebouncedTextFilter({
  field,
  updateParams,
  className,
}: FieldControlProps<Extract<FilterField, { type: "search" | "text" }>>) {
  const { value, setValue, applied, markApplied } = useParamValue(field.name);
  const isSearch = field.type === "search";

  useEffect(() => {
    const trimmed = value.trim();
    if (trimmed === applied) {
      return;
    }
    const timer = setTimeout(() => {
      markApplied(trimmed);
      updateParams({ [field.name]: trimmed });
    }, DEBOUNCE_MS);
    return () => clearTimeout(timer);
  }, [value, applied, markApplied, updateParams, field.name]);

  const input = (
    <InputGroup className={isSearch ? className : undefined}>
      {isSearch && (
        <InputGroupAddon>
          <Search aria-hidden />
        </InputGroupAddon>
      )}
      <InputGroupInput
        type="search"
        enterKeyHint="search"
        aria-label={field.label}
        placeholder={field.placeholder ?? field.label}
        maxLength={field.maxLength}
        value={value}
        onChange={(event) => setValue(event.target.value)}
      />
      {value && (
        <InputGroupAddon align="inline-end">
          <InputGroupButton size="icon-sm" aria-label="Borrar búsqueda" onClick={() => setValue("")}>
            <X aria-hidden />
          </InputGroupButton>
        </InputGroupAddon>
      )}
    </InputGroup>
  );

  if (isSearch) {
    return input;
  }

  return (
    <Field className={className}>
      <FieldLabel className="text-xs text-muted-foreground">{field.label}</FieldLabel>
      {input}
    </Field>
  );
}

function ImmediateFilter({ field, updateParams }: FieldControlProps<Extract<FilterField, { type: "select" | "date" }>>) {
  const { value, setValue, markApplied } = useParamValue(field.name);

  const apply = (next: string) => {
    setValue(next);
    markApplied(next);
    updateParams({ [field.name]: next });
  };

  return (
    <Field className={field.type === "select" ? "w-48" : "w-44"}>
      <FieldLabel htmlFor={`filter-${field.name}`} className="text-xs text-muted-foreground">
        {field.label}
      </FieldLabel>
      {field.type === "select" ? (
        <Select value={value || ALL} onValueChange={(next) => apply(next === ALL ? "" : next)}>
          <SelectTrigger id={`filter-${field.name}`} className="w-full">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={ALL}>{field.allLabel ?? "Todos"}</SelectItem>
            {field.options.map((option) => (
              <SelectItem key={option.value} value={option.value}>
                {option.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      ) : (
        <Input
          id={`filter-${field.name}`}
          type="date"
          value={value}
          onChange={(event) => apply(event.target.value)}
        />
      )}
    </Field>
  );
}

type MobileFiltersProps = {
  fields: readonly FilterField[];
  activeCount: number;
  updateParams: (changes: Record<string, string>) => void;
};

/** Filters in a bottom sheet with a draft that is applied on "Aplicar". */
function MobileFilters({ fields, activeCount, updateParams }: MobileFiltersProps) {
  const searchParams = useSearchParams();
  const [open, setOpen] = useState(false);
  const [draft, setDraft] = useState<Record<string, string>>({});

  const openSheet = () => {
    setDraft(Object.fromEntries(fields.map((field) => [field.name, searchParams.get(field.name) ?? ""])));
    setOpen(true);
  };

  const setField = (name: string, value: string) => setDraft((current) => ({ ...current, [name]: value }));

  const apply = (values: Record<string, string>) => {
    updateParams(values);
    setOpen(false);
  };

  return (
    <>
      <Button variant="outline" onClick={openSheet} className="md:hidden" aria-label="Filtros">
        <SlidersHorizontal aria-hidden />
        <span>Filtros</span>
        {activeCount > 0 && (
          <Badge className="ml-0.5 size-5 justify-center rounded-full p-0 tabular-nums">{activeCount}</Badge>
        )}
      </Button>

      <Sheet open={open} onOpenChange={setOpen}>
        <SheetContent side="bottom" className="max-h-[85dvh] rounded-t-2xl pb-[env(safe-area-inset-bottom)]">
          <SheetHeader>
            <SheetTitle>Filtros</SheetTitle>
            <SheetDescription className="sr-only">Ajusta los filtros del listado</SheetDescription>
          </SheetHeader>
          <div className="flex flex-col gap-4 overflow-y-auto px-4">
            {fields.map((field) => (
              <Field key={field.name}>
                <FieldLabel htmlFor={`mobile-filter-${field.name}`}>{field.label}</FieldLabel>
                {field.type === "select" ? (
                  <Select
                    value={draft[field.name] || ALL}
                    onValueChange={(next) => setField(field.name, next === ALL ? "" : next)}
                  >
                    <SelectTrigger id={`mobile-filter-${field.name}`} className="w-full">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={ALL}>{field.allLabel ?? "Todos"}</SelectItem>
                      {field.options.map((option) => (
                        <SelectItem key={option.value} value={option.value}>
                          {option.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                ) : (
                  <Input
                    id={`mobile-filter-${field.name}`}
                    type={field.type === "date" ? "date" : "text"}
                    maxLength={field.type === "date" ? undefined : field.maxLength}
                    placeholder={field.type === "date" ? undefined : field.placeholder}
                    value={draft[field.name] ?? ""}
                    onChange={(event) => setField(field.name, event.target.value)}
                  />
                )}
              </Field>
            ))}
          </div>
          <SheetFooter className="flex-row gap-2">
            <Button
              variant="outline"
              className="flex-1"
              onClick={() => apply(Object.fromEntries(fields.map((field) => [field.name, ""])))}
            >
              Limpiar
            </Button>
            <Button className="flex-1" onClick={() => apply(draft)}>
              Aplicar
            </Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>
    </>
  );
}
