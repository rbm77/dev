/** Every expense list (GET /expenses, /fuel-expenses, ...) only carries these columns. */
export type ExpenseListItem = { id: number; date?: string; amount: number };

/** GET {endpoint}/{id}: base fields plus the reference of the subtype. */
export type Expense = ExpenseListItem & {
  description?: string;
  vehicleId?: number;
  driverId?: number;
  liters?: number;
  maintenanceId?: number;
  employeeId?: number;
  incidentId?: number;
};
