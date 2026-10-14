import "server-only";

import type { LucideIcon } from "lucide-react";
import { HandCoins, Inbox, Route, Wrench } from "lucide-react";
import { ApiError, apiGet, type Paged, type Query } from "@/lib/api";
import { can, type Permission, type Resource } from "@/lib/permissions";

export type Counter = {
  key: string;
  label: string;
  description: string;
  href: string;
  icon: LucideIcon;
  count: number;
  tone: "warning" | "danger";
};

type CounterDefinition = Omit<Counter, "count"> & { resource: Resource; path: string; query?: Query };

// Only totalCount is needed, so each list is requested with pageSize=1.
const DEFINITIONS: CounterDefinition[] = [
  {
    key: "payment-requests",
    label: "Solicitudes por validar",
    description: "Comprobantes de pago recibidos",
    href: "/payment-requests?isValidated=false",
    icon: Inbox,
    tone: "warning",
    resource: "PAYMENT_REQUEST",
    path: "/payment-requests",
    query: { isValidated: false },
  },
  {
    key: "debtors",
    label: "Estudiantes con deuda",
    description: "Estudiantes activos con saldo pendiente",
    href: "/debtors?isActive=true",
    icon: HandCoins,
    tone: "danger",
    resource: "DEBTOR",
    path: "/reports/debtors",
    query: { isActive: true },
  },
  {
    key: "maintenances",
    label: "Mantenimientos pendientes",
    description: "Programados y aún no realizados",
    href: "/maintenances",
    icon: Wrench,
    tone: "warning",
    resource: "MAINTENANCE",
    path: "/maintenances/pending",
  },
  {
    key: "custom-transports",
    label: "Transportes pendientes",
    description: "Servicios especiales por realizar",
    href: "/custom-transports",
    icon: Route,
    tone: "warning",
    resource: "CUSTOM_TRANSPORT",
    path: "/custom-transports/pending",
  },
];

async function count(definition: CounterDefinition): Promise<number | null> {
  try {
    const page = await apiGet<Paged<unknown>>(definition.path, { ...definition.query, page: 1, pageSize: 1 });
    return page.totalCount;
  } catch (error) {
    if (error instanceof ApiError) {
      return null;
    }
    throw error;
  }
}

/** Actionable counters for the home page, limited to what the user can read. Failed ones are skipped. */
export async function getPendingCounters(permissions: readonly Permission[]): Promise<Counter[]> {
  const visible = DEFINITIONS.filter((definition) => can(permissions, definition.resource, "READ"));
  const counts = await Promise.all(visible.map(count));
  return visible.flatMap((definition, index) => {
    const value = counts[index];
    if (value === null) {
      return [];
    }
    const { key, label, description, href, icon, tone } = definition;
    return [{ key, label, description, href, icon, tone, count: value }];
  });
}

export function hasCounterPermissions(permissions: readonly Permission[]): boolean {
  return DEFINITIONS.some((definition) => can(permissions, definition.resource, "READ"));
}
