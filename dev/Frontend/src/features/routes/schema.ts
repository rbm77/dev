import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { optionalText, requiredText } from "@/lib/validation";

/** POST /routes, PUT /routes/{id} */
export const routeSchema = z.object({
  name: requiredText(LIMITS.route.name),
  description: optionalText(LIMITS.route.description),
  isActive: z.boolean(),
});

export type RouteFormValues = z.input<typeof routeSchema>;
