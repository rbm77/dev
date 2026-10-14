/** GET /payment-period-requests — the list only carries the start date. */
export type PaymentPeriodRequestListItem = { id: number; startDate?: string };

/** GET /payment-period-requests/{id} */
export type PaymentPeriodRequest = { id: number; amount: number; startDate?: string; daysToNextPayment: number };
