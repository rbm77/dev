import "server-only";

import type * as z from "zod";
import { apiCreate, apiGet, apiGetOptional, apiSend, type Paged } from "@/lib/api";
import type { paymentSchema } from "./schema";
import type { Debtor, Payment, PaymentListItem, PaymentRequest } from "./types";

// Payments
export type PaymentFilters = { date?: string; studentId?: number; receiptReference?: string; page: number; pageSize: number };

export function listPayments(filters: PaymentFilters) {
  return apiGet<Paged<PaymentListItem>>("/payments", filters);
}

export function getPayment(id: number) {
  return apiGetOptional<Payment>(`/payments/${id}`);
}

export function createPayment(input: z.output<typeof paymentSchema>) {
  return apiCreate("/payments", input);
}

export function updatePayment(id: number, input: z.output<typeof paymentSchema>) {
  return apiSend("PUT", `/payments/${id}`, input);
}

export function deletePayment(id: number) {
  return apiSend("DELETE", `/payments/${id}`);
}

// Payment requests (created by the SMS/e-mail/WhatsApp ingestion services)
export type PaymentRequestFilters = {
  requestedAt?: string;
  isValidated?: boolean;
  receiptReference?: string;
  page: number;
  pageSize: number;
};

export function listPaymentRequests(filters: PaymentRequestFilters) {
  return apiGet<Paged<PaymentRequest>>("/payment-requests", filters);
}

export function validatePaymentRequest(id: number) {
  return apiSend("POST", `/payment-requests/${id}/validate`);
}

/** 201 + Location: /Payments/{newId}. Returns the new payment id. */
export function approvePaymentRequest(id: number) {
  return apiCreate(`/payment-requests/${id}/approve`, undefined);
}

export function rejectPaymentRequest(id: number) {
  return apiSend("POST", `/payment-requests/${id}/reject`);
}

// Debtors report
export type DebtorFilters = { routeId?: number; studentId?: number; isActive?: boolean; page: number; pageSize: number };

export function listDebtors(filters: DebtorFilters) {
  return apiGet<Paged<Debtor>>("/reports/debtors", filters);
}
