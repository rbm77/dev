// API contract of the catalog modules (Features/Routes, Grades, Roles, Vehicles,
// Employees, Maintenances, Incidents, PaymentPeriods, Students). Optional
// properties are the ones the API omits when null (WhenWritingNull).

/** GET /routes, GET /routes/{id} */
export type Route = { id: number; name?: string; description?: string; isActive: boolean };

/** GET /grades, GET /grades/{id} */
export type Grade = { id: number; description?: string };

/** GET /roles, GET /roles/{id} */
export type Role = { id: number; description?: string };

/** GET /roles/{id}/permissions */
export type RolePermission = { resourceId: number; description?: string; isAssigned: boolean; isEditable: boolean };

/** GET /vehicles — the list only carries these columns. */
export type VehicleListItem = { id: number; licensePlate?: string; make?: string; model?: string; isActive: boolean };

/** GET /vehicles/{id} */
export type Vehicle = VehicleListItem & {
  manufactureYear: number;
  capacity: number;
  mileage: number;
  acquisitionDate?: string;
};

/** GET /employees — `driverDetails` only has the license number in the list. */
export type EmployeeListItem = {
  id: number;
  hireDate?: string;
  isActive: boolean;
  identityDocument?: string;
  name?: string;
  lastName?: string;
  driverDetails?: { licenseNumber?: string };
};

/** GET /employees/{id} */
export type Employee = Omit<EmployeeListItem, "driverDetails"> & {
  address?: string;
  phoneNumber?: string;
  email?: string;
  driverDetails?: { licenseNumber?: string; licenseExpiryDate?: string };
};

/** GET /students — the list has no address. */
export type StudentListItem = {
  id: number;
  name?: string;
  lastName?: string;
  identityDocument?: string;
  routeId: number;
  gradeId: number;
  isActive: boolean;
};

/** GET /students/{id} */
export type Student = StudentListItem & { address?: string };

/** GET /maintenances/pending */
export type PendingMaintenance = { id: number; vehicleId: number; type: number; scheduledDate?: string };

/** GET /maintenances/completed */
export type CompletedMaintenance = { id: number; vehicleId: number; type: number; completedDate?: string };

/** GET /maintenances/{id} */
export type Maintenance = {
  id: number;
  vehicleId: number;
  description?: string;
  type: number;
  scheduledDate?: string;
  completedDate?: string;
};

/** GET /incidents — 0 in vehicleId/driverId means "not assigned" (nullable columns). */
export type IncidentListItem = { id: number; vehicleId: number; driverId: number; date?: string; type: number };

/** GET /incidents/{id} */
export type Incident = IncidentListItem & { description?: string; correctiveActions?: string };

/** GET /payment-periods */
export type PaymentPeriod = { id: number; requestId: number; paymentDate?: string };
