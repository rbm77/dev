import "server-only";

import { cache } from "react";
import type { Option } from "@/components/forms/fields";
import { ApiError, apiGet, apiGetOptional, type Paged } from "@/lib/api";
import { formatDate } from "@/lib/dates";
import { incidentTypeLabel, maintenanceTypeLabel } from "@/lib/enums";
import { fullName } from "@/lib/format";
import { can, type Resource } from "@/lib/permissions";
import { getSession } from "@/lib/session";
import type {
  CompletedMaintenance,
  EmployeeListItem,
  Grade,
  IncidentListItem,
  PaymentPeriod,
  PendingMaintenance,
  Role,
  Route,
  Student,
  VehicleListItem,
} from "./types";

// Options for selects and id → name maps. Catalogs are small, so they are read
// whole (once per render thanks to React cache). If the user cannot read a
// catalog, an empty list is returned and the UI falls back to "#id".

const CATALOG_PAGE_SIZE = 500;

async function readable(resource: Resource): Promise<boolean> {
  const session = await getSession();
  return session !== null && can(session.permissions, resource, "READ");
}

async function fetchAll<T>(resource: Resource, path: string, query: Record<string, string | number> = {}): Promise<T[]> {
  if (!(await readable(resource))) {
    return [];
  }
  try {
    const page = await apiGet<Paged<T>>(path, { ...query, page: 1, pageSize: CATALOG_PAGE_SIZE });
    return page.items;
  } catch (error) {
    if (error instanceof ApiError) {
      return [];
    }
    throw error;
  }
}

export function toLabelMap(options: readonly Option[]): Map<string, string> {
  return new Map(options.map((option) => [option.value, option.label]));
}

/** Name for an id, or "#id" when it is not in the map (0 means "none"). */
export function labelOf(map: Map<string, string>, id: number | null | undefined): string {
  if (!id) {
    return "—";
  }
  return map.get(String(id)) ?? `#${id}`;
}

const inactive = (isActive: boolean) => (isActive ? "" : " (inactivo)");

export const getRouteOptions = cache(async (): Promise<Option[]> =>
  (await fetchAll<Route>("ROUTE", "/routes")).map((route) => ({
    value: String(route.id),
    label: `${route.name ?? `Ruta #${route.id}`}${inactive(route.isActive)}`,
  })),
);

export const getGradeOptions = cache(async (): Promise<Option[]> =>
  (await fetchAll<Grade>("GRADE", "/grades")).map((grade) => ({
    value: String(grade.id),
    label: grade.description ?? `Grado #${grade.id}`,
  })),
);

export const getRoleOptions = cache(async (): Promise<Option[]> =>
  (await fetchAll<Role>("ROLE", "/roles")).map((role) => ({
    value: String(role.id),
    label: role.description ?? `Rol #${role.id}`,
  })),
);

const getVehicles = cache(() => fetchAll<VehicleListItem>("VEHICLE", "/vehicles"));

export const getVehicleOptions = cache(async (): Promise<Option[]> =>
  (await getVehicles()).map((vehicle) => ({
    value: String(vehicle.id),
    label: `${vehicle.licensePlate ?? `#${vehicle.id}`} · ${[vehicle.make, vehicle.model].filter(Boolean).join(" ")}${inactive(vehicle.isActive)}`,
  })),
);

/** id → license plate, for compact labels in lists. */
export const getVehiclePlates = cache(async (): Promise<Map<string, string>> =>
  new Map((await getVehicles()).map((vehicle) => [String(vehicle.id), vehicle.licensePlate ?? `#${vehicle.id}`])),
);

const getEmployees = cache(() => fetchAll<EmployeeListItem>("EMPLOYEE", "/employees"));

export const getEmployeeOptions = cache(async (): Promise<Option[]> =>
  (await getEmployees()).map((employee) => ({
    value: String(employee.id),
    label: `${fullName(employee.name, employee.lastName)}${inactive(employee.isActive)}`,
  })),
);

/** Drivers are the employees that have driver details (license). */
export const getDriverOptions = cache(async (): Promise<Option[]> =>
  (await getEmployees())
    .filter((employee) => employee.driverDetails?.licenseNumber)
    .map((employee) => ({
      value: String(employee.id),
      label: `${fullName(employee.name, employee.lastName)}${inactive(employee.isActive)}`,
    })),
);

export const getMaintenanceOptions = cache(async (): Promise<Option[]> => {
  const [pending, completed, plates] = await Promise.all([
    fetchAll<PendingMaintenance>("MAINTENANCE", "/maintenances/pending"),
    fetchAll<CompletedMaintenance>("MAINTENANCE", "/maintenances/completed"),
    getVehiclePlates(),
  ]);
  return [
    ...pending.map((item) => ({ id: item.id, vehicleId: item.vehicleId, type: item.type, date: item.scheduledDate })),
    ...completed.map((item) => ({ id: item.id, vehicleId: item.vehicleId, type: item.type, date: item.completedDate })),
  ]
    .sort((a, b) => (b.date ?? "").localeCompare(a.date ?? ""))
    .map((item) => ({
      value: String(item.id),
      label: `${labelOf(plates, item.vehicleId)} · ${maintenanceTypeLabel(item.type)} · ${formatDate(item.date)}`,
    }));
});

export const getIncidentOptions = cache(async (): Promise<Option[]> => {
  const [incidents, plates] = await Promise.all([fetchAll<IncidentListItem>("INCIDENT", "/incidents"), getVehiclePlates()]);
  return incidents.map((incident) => ({
    value: String(incident.id),
    label: `${formatDate(incident.date)} · ${incidentTypeLabel(incident.type)} · ${labelOf(plates, incident.vehicleId)}`,
  }));
});

export const getPaymentPeriodOptions = cache(async (): Promise<Option[]> =>
  (await fetchAll<PaymentPeriod>("PAYMENT_PERIOD", "/payment-periods"))
    .sort((a, b) => (b.paymentDate ?? "").localeCompare(a.paymentDate ?? ""))
    .map((period) => ({ value: String(period.id), label: `Cobro del ${formatDate(period.paymentDate)}` })),
);

const getStudent = cache(async (id: number): Promise<Student | null> => {
  try {
    return await apiGetOptional<Student>(`/students/${id}`);
  } catch (error) {
    if (error instanceof ApiError) {
      return null;
    }
    throw error;
  }
});

/**
 * Names of the students referenced by a page of results (at most one request
 * per distinct id, in parallel). Lists only return studentId.
 */
export async function getStudentNames(ids: readonly number[]): Promise<Map<string, string>> {
  if (!(await readable("STUDENT"))) {
    return new Map();
  }
  const unique = [...new Set(ids.filter((id) => Number.isSafeInteger(id) && id > 0))];
  const students = await Promise.all(unique.map((id) => getStudent(id)));
  return new Map(
    students
      .filter((student): student is Student => student !== null)
      .map((student) => [String(student.id), fullName(student.name, student.lastName)]),
  );
}

export async function getStudentName(id: number): Promise<string | undefined> {
  return (await getStudentNames([id])).get(String(id));
}
