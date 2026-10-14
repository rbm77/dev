"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { ApiError } from "@/lib/api";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { rolePermissionsSchema, roleSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createRole(input: unknown): Promise<ActionResult> {
  const parsed = roleSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("ROLE", "WRITE");
    await api.createRole(parsed.data);
    revalidatePath("/settings/roles");
  });
}

export async function updateRole(id: unknown, input: unknown): Promise<ActionResult> {
  const roleId = parseId(id);
  if (!roleId) return INVALID_ID;
  const parsed = roleSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("ROLE", "WRITE");
    await api.updateRole(roleId, parsed.data);
    revalidatePath("/settings/roles");
  });
}

export async function deleteRole(id: unknown): Promise<ActionResult> {
  const roleId = parseId(id);
  if (!roleId) return INVALID_ID;

  return runAction(async () => {
    await authorize("ROLE", "WRITE");
    await api.deleteRole(roleId);
    revalidatePath("/settings/roles");
  });
}

export async function saveRolePermissions(id: unknown, permissions: unknown): Promise<ActionResult> {
  const roleId = parseId(id);
  if (!roleId) return INVALID_ID;
  const parsed = rolePermissionsSchema.safeParse(permissions);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("ROLE", "WRITE");
    try {
      await api.updateRolePermissions(roleId, parsed.data);
    } catch (error) {
      // The procedure reports 0 affected rows (404) when nothing changed.
      if (!(error instanceof ApiError && error.status === 404)) throw error;
    }
    revalidatePath(`/settings/roles/${roleId}`);
  });
}
