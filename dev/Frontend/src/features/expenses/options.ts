import "server-only";

import {
  getDriverOptions,
  getEmployeeOptions,
  getIncidentOptions,
  getMaintenanceOptions,
  getVehicleOptions,
} from "@/features/catalogs/lookups";
import type { ExpenseOptions } from "./components/expense-form";
import type { ExpenseKind } from "./kinds";

/** Select options needed by the form and filters of each expense kind. */
export async function getExpenseOptions(kind: ExpenseKind): Promise<ExpenseOptions> {
  switch (kind) {
    case "fuel": {
      const [vehicles, drivers] = await Promise.all([getVehicleOptions(), getDriverOptions()]);
      return { vehicles, drivers };
    }
    case "maintenance":
      return { maintenances: await getMaintenanceOptions() };
    case "salary":
      return { employees: await getEmployeeOptions() };
    case "incident":
      return { incidents: await getIncidentOptions() };
    default:
      return {};
  }
}
