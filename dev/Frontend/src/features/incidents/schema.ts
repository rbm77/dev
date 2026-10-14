import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { INCIDENT_TYPES } from "@/lib/enums";
import { entityId, enumValue, isoDate, optionalText } from "@/lib/validation";

/** POST /incidents, PUT /incidents/{id} */
export const incidentSchema = z.object({
  date: isoDate(),
  type: enumValue(INCIDENT_TYPES.length),
  vehicleId: entityId(),
  driverId: entityId(),
  description: optionalText(LIMITS.incident.description),
  correctiveActions: optionalText(LIMITS.incident.correctiveActions),
});

export type IncidentFormValues = z.input<typeof incidentSchema>;
