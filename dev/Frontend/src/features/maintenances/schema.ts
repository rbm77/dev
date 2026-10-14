import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { MAINTENANCE_TYPES } from "@/lib/enums";
import { entityId, enumValue, isoDate, optionalText } from "@/lib/validation";

/** POST /maintenances, PUT /maintenances/{id} while it is pending. */
export const maintenanceSchema = z.object({
  vehicleId: entityId(),
  type: enumValue(MAINTENANCE_TYPES.length),
  scheduledDate: isoDate(),
  description: optionalText(LIMITS.maintenance.description),
});

/**
 * PUT /maintenances/{id} once completed: completing deletes the scheduled date
 * in the backend, and a null scheduledDate tells it to leave that date alone.
 */
export const completedMaintenanceSchema = maintenanceSchema.extend({
  scheduledDate: z.literal("").transform(() => null),
});

export type MaintenanceFormValues = z.input<typeof maintenanceSchema>;
