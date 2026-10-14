const moneyFormatter = new Intl.NumberFormat("es-CR", {
  style: "currency",
  currency: "CRC",
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

const numberFormatter = new Intl.NumberFormat("es-CR", { maximumFractionDigits: 2 });

/** Amount in Costa Rican colones, e.g. ₡1 250,00. */
export function formatMoney(value: number | null | undefined): string {
  return value === null || value === undefined ? "—" : moneyFormatter.format(value);
}

export function formatNumber(value: number | null | undefined): string {
  return value === null || value === undefined ? "—" : numberFormatter.format(value);
}

export function formatPercent(value: number | null | undefined): string {
  return value === null || value === undefined ? "—" : `${numberFormatter.format(value)} %`;
}

export function fullName(name?: string | null, lastName?: string | null): string {
  return [name, lastName].filter(Boolean).join(" ") || "—";
}
