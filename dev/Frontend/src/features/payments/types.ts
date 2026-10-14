/** GET /payments — the list does not include the amount. */
export type PaymentListItem = { id: number; date?: string; studentId: number; receiptReference?: string };

/** GET /payments/{id} */
export type Payment = PaymentListItem & { amount: number };

/** GET /payment-requests and /payment-requests/{id} (full row in both). */
export type PaymentRequest = {
  id: number;
  date?: string;
  amount: number;
  studentId: number;
  receiptReference: string;
  requestedAt?: string;
  isValidated: boolean;
};

/** GET /reports/debtors — `id` is the student id. */
export type Debtor = {
  id: number;
  name: string;
  lastName: string;
  identityDocument?: string;
  gradeId: number;
  routeId: number;
  entryDate: string;
  isActive: boolean;
  dueAmount: number;
  periodsCount: number;
  paymentsCount: number;
};
