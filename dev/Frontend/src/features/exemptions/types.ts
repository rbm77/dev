/** GET /specific-exemptions — no description or percentage in the list. */
export type SpecificExemptionListItem = { id: number; studentId: number; paymentPeriodId: number };

/** GET /specific-exemptions/{id} */
export type SpecificExemption = SpecificExemptionListItem & { description?: string; percentage: number };

/** GET /periodic-exemptions — no description or dates in the list. */
export type PeriodicExemptionListItem = { id: number; studentId: number; percentage: number };

/** GET /periodic-exemptions/{id} */
export type PeriodicExemption = PeriodicExemptionListItem & { description?: string; startDate?: string; endDate?: string };
