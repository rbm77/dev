"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Check, ChevronsUpDown, Loader2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { cn } from "@/lib/utils";

export type ComboboxItem = { value: string; label: string; detail?: string };

type EntityComboboxProps = {
  /** Route Handler that answers `?q=` with `{ items: ComboboxItem[] }`. */
  endpoint: string;
  value: string;
  selectedLabel?: string;
  onSelect: (item: ComboboxItem | null) => void;
  placeholder?: string;
  searchPlaceholder?: string;
  allowClear?: boolean;
  disabled?: boolean;
  invalid?: boolean;
  id?: string;
  /** Accessible name when there is no <label> pointing to `id`. */
  label?: string;
};

const MIN_QUERY = 2;
const DEBOUNCE_MS = 300;

/** Searchable select backed by the server (used for students and users). */
export function EntityCombobox({
  endpoint,
  value,
  selectedLabel,
  onSelect,
  placeholder = "Buscar…",
  searchPlaceholder = "Escribe para buscar",
  allowClear = false,
  disabled,
  invalid,
  id,
  label,
}: EntityComboboxProps) {
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [items, setItems] = useState<ComboboxItem[]>([]);
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  useEffect(() => {
    if (!open) {
      return;
    }
    const controller = new AbortController();
    const timer = setTimeout(async () => {
      const term = query.trim();
      if (term.length < MIN_QUERY) {
        setItems([]);
        setLoading(false);
        return;
      }
      setLoading(true);
      try {
        const response = await fetch(`${endpoint}?q=${encodeURIComponent(term)}`, {
          headers: { Accept: "application/json" },
          signal: controller.signal,
        });
        // 401, or a redirect issued because the API rejected the token.
        if (response.status === 401 || response.redirected) {
          router.replace("/session-expired");
          return;
        }
        const data = response.ok ? ((await response.json()) as { items: ComboboxItem[] }) : { items: [] };
        setItems(data.items);
      } catch {
        if (!controller.signal.aborted) {
          setItems([]);
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false);
        }
      }
    }, DEBOUNCE_MS);

    return () => {
      clearTimeout(timer);
      controller.abort();
    };
  }, [open, query, endpoint, router]);

  const choose = (item: ComboboxItem | null) => {
    onSelect(item);
    setOpen(false);
    setQuery("");
  };

  const emptyText = query.trim().length < MIN_QUERY ? "Escribe al menos 2 caracteres." : "Sin resultados.";

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          id={id}
          type="button"
          variant="outline"
          role="combobox"
          aria-label={label}
          aria-expanded={open}
          aria-invalid={invalid}
          disabled={disabled}
          className={cn(
            "w-full justify-between font-normal",
            !value && "text-muted-foreground",
            invalid && "border-destructive",
          )}
        >
          <span className="truncate">{value ? (selectedLabel ?? `#${value}`) : placeholder}</span>
          <ChevronsUpDown className="opacity-50" aria-hidden />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-(--radix-popover-trigger-width) min-w-64 p-0" align="start">
        <Command shouldFilter={false}>
          <CommandInput
            value={query}
            onValueChange={setQuery}
            placeholder={searchPlaceholder}
            maxLength={50}
            className="text-base md:text-sm"
          />
          <CommandList>
            {loading ? (
              <div className="flex items-center justify-center gap-2 py-6 text-sm text-muted-foreground">
                <Loader2 className="size-4 animate-spin" aria-hidden />
                Buscando…
              </div>
            ) : (
              <CommandEmpty>{emptyText}</CommandEmpty>
            )}
            {!loading && (
              <CommandGroup>
                {allowClear && value && (
                  <CommandItem value="__clear__" onSelect={() => choose(null)} className="text-muted-foreground">
                    <X aria-hidden />
                    Quitar selección
                  </CommandItem>
                )}
                {items.map((item) => (
                  <CommandItem key={item.value} value={item.value} onSelect={() => choose(item)} className="py-2.5 md:py-1.5">
                    <Check className={cn(item.value === value ? "opacity-100" : "opacity-0")} aria-hidden />
                    <span className="flex min-w-0 flex-col">
                      <span className="truncate">{item.label}</span>
                      {item.detail && <span className="truncate text-xs text-muted-foreground">{item.detail}</span>}
                    </span>
                  </CommandItem>
                ))}
              </CommandGroup>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
