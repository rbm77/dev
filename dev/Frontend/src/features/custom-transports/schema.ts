import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { entityId, isoDate, money, optionalText } from "@/lib/validation";

/** POST /custom-transports, PUT /custom-transports/{id} while it is pending. */
export const customTransportSchema = z.object({
  scheduledDate: isoDate(),
  vehicleId: entityId(),
  driverId: entityId(),
  amount: money(),
  description: optionalText(LIMITS.customTransport.description),
});

/**
 * PUT /custom-transports/{id} once completed: completing deletes the scheduled
 * date in the backend, and a null scheduledDate tells it to leave that date alone.
 */
export const completedCustomTransportSchema = customTransportSchema.extend({
  scheduledDate: z.literal("").transform(() => null),
});

export type CustomTransportFormValues = z.input<typeof customTransportSchema>;
