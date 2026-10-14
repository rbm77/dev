import * as z from "zod";
import { LIMITS } from "@/lib/db-limits";
import { requiredText } from "@/lib/validation";

/** POST /roles, PUT /roles/{id} */
export const roleSchema = z.object({
  description: requiredText(LIMITS.role.description),
});

export type RoleFormValues = z.input<typeof roleSchema>;

/** PUT /roles/{id}/permissions — only the assigned resources; replaces the whole set. */
export const rolePermissionsSchema = z
  .array(z.object({ resourceId: z.number().int().positive(), isEditable: z.boolean() }))
  .max(100);
