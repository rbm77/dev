import type { TabLink } from "@/components/shared/tab-links";
import { can, type Permission } from "@/lib/permissions";

/** "Específicas | Periódicas" tabs, only for the kinds the user can read. Keeps the student filter. */
export function exemptionTabs(permissions: readonly Permission[], studentId?: number): TabLink[] {
  const query = studentId ? `?studentId=${studentId}` : "";
  const tabs: TabLink[] = [];
  if (can(permissions, "SPECIFIC_EXEMPTION", "READ")) {
    tabs.push({ value: "specific", label: "Específicas", href: `/exemptions/specific${query}` });
  }
  if (can(permissions, "PERIODIC_EXEMPTION", "READ")) {
    tabs.push({ value: "periodic", label: "Periódicas", href: `/exemptions/periodic${query}` });
  }
  return tabs;
}
