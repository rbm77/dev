// The API exchanges every date as "yyyy-MM-dd" (DateOnlyJsonConverter).
// Dates are handled as plain strings so no time zone can shift the day.

const TIME_ZONE = "America/Costa_Rica";
const ISO_DATE = /^(\d{4})-(\d{2})-(\d{2})$/;

/** Today in Costa Rica as "yyyy-MM-dd", independent of the server time zone. */
export function todayIso(): string {
  return new Intl.DateTimeFormat("en-CA", {
    timeZone: TIME_ZONE,
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).format(new Date());
}

/** True for a real calendar date in "yyyy-MM-dd" format. */
export function isIsoDate(value: string): boolean {
  const match = ISO_DATE.exec(value);
  if (!match) {
    return false;
  }
  const [, year, month, day] = match.map(Number);
  const date = new Date(Date.UTC(year, month - 1, day));
  return date.getUTCFullYear() === year && date.getUTCMonth() === month - 1 && date.getUTCDate() === day;
}

/** "yyyy-MM-dd" → "dd/mm/aaaa". Returns "—" when there is no date. */
export function formatDate(value: string | null | undefined): string {
  if (!value) {
    return "—";
  }
  const match = ISO_DATE.exec(value);
  return match ? `${match[3]}/${match[2]}/${match[1]}` : "—";
}
