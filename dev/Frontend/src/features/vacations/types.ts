/** GET /vacation — the list has no description. */
export type VacationListItem = { id: number; startDate?: string; endDate?: string };

/** GET /vacation/{id} */
export type Vacation = VacationListItem & { description?: string };
