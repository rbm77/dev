// The five expense endpoints share date/amount/description; each subtype adds
// its own reference (Features/Expenses, FuelExpenses, MaintenanceExpenses,
// SalaryExpenses, IncidentExpenses). All require the EXPENSE permission.

export const EXPENSE_KINDS = ["general", "fuel", "maintenance", "salary", "incident"] as const;
export type ExpenseKind = (typeof EXPENSE_KINDS)[number];

type KindInfo = {
  /** Tab label. */
  label: string;
  /** Noun used in titles and messages. */
  noun: string;
  /** API base path. */
  endpoint: string;
  /** Only general expenses can be deleted (DELETE /expenses/{id}). */
  deletable: boolean;
};

export const EXPENSE_KIND_INFO: Readonly<Record<ExpenseKind, KindInfo>> = {
  general: { label: "Generales", noun: "gasto general", endpoint: "/expenses", deletable: true },
  fuel: { label: "Combustible", noun: "gasto de combustible", endpoint: "/fuel-expenses", deletable: false },
  maintenance: { label: "Mantenimiento", noun: "gasto de mantenimiento", endpoint: "/maintenance-expenses", deletable: false },
  salary: { label: "Salarios", noun: "pago de salario", endpoint: "/salary-expenses", deletable: false },
  incident: { label: "Incidentes", noun: "gasto por incidente", endpoint: "/incident-expenses", deletable: false },
};

export function toExpenseKind(value: string): ExpenseKind | null {
  return EXPENSE_KINDS.find((kind) => kind === value) ?? null;
}
