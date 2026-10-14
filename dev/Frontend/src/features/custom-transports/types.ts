// Features/CustomTransports contract. The lists do not include the amount.

/** GET /custom-transports/pending */
export type PendingCustomTransport = { id: number; vehicleId: number; driverId: number; scheduledDate?: string };

/** GET /custom-transports/completed */
export type CompletedCustomTransport = { id: number; vehicleId: number; driverId: number; completedDate?: string };

/** GET /custom-transports/{id} */
export type CustomTransport = {
  id: number;
  vehicleId: number;
  driverId: number;
  amount: number;
  description?: string;
  scheduledDate?: string;
  completedDate?: string;
};
