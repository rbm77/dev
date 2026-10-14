import * as z from "zod";
import { LIMITS, NUMBER_LIMITS } from "@/lib/db-limits";
import { integer, isoDate, requiredText } from "@/lib/validation";

const nextYear = new Date().getFullYear() + 1;

/** POST /vehicles, PUT /vehicles/{id}. Plates are stored uppercase (the list filters them by exact match). */
export const vehicleSchema = z.object({
  licensePlate: requiredText(LIMITS.vehicle.licensePlate).transform((plate) => plate.toUpperCase()),
  make: requiredText(LIMITS.vehicle.make),
  model: requiredText(LIMITS.vehicle.model),
  manufactureYear: integer(NUMBER_LIMITS.manufactureYear.min, nextYear),
  capacity: integer(NUMBER_LIMITS.vehicleCapacity.min, NUMBER_LIMITS.vehicleCapacity.max),
  mileage: integer(0),
  acquisitionDate: isoDate(),
  isActive: z.boolean(),
});

export type VehicleFormValues = z.input<typeof vehicleSchema>;
