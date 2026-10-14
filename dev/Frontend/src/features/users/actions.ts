"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { ApiError } from "@/lib/api";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { changePasswordSchema, criticalUsersSchema, ownUserSchema, userSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createUser(input: unknown): Promise<ActionResult<number | null>> {
  const parsed = userSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("USER", "WRITE");
    const id = await api.createUser(parsed.data);
    revalidatePath("/settings/users");
    return id;
  });
}

export async function updateUser(id: unknown, input: unknown): Promise<ActionResult> {
  const userId = parseId(id);
  if (!userId) return INVALID_ID;
  const parsed = userSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("USER", "WRITE");
    await api.updateUser(userId, parsed.data);
    revalidatePath("/settings/users");
  });
}

export async function deleteUser(id: unknown): Promise<ActionResult> {
  const userId = parseId(id);
  if (!userId) return INVALID_ID;

  return runAction(async () => {
    await authorize("USER", "WRITE");
    await api.deleteUser(userId);
    revalidatePath("/settings/users");
  });
}

/** The user id always comes from the session, never from the client. */
export async function updateProfile(input: unknown): Promise<ActionResult> {
  const parsed = ownUserSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    const session = await authorize("OWN_USER", "WRITE");
    await api.updateOwnUser(session.userId, parsed.data);
    revalidatePath("/profile");
  });
}

export async function changePassword(input: unknown): Promise<ActionResult> {
  const parsed = changePasswordSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    const session = await authorize("OWN_USER", "WRITE");
    await api.changeOwnPassword(session.userId, parsed.data.password);
  });
}

export async function saveCriticalProcessUsers(ids: unknown): Promise<ActionResult> {
  const parsed = criticalUsersSchema.safeParse(ids);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("USER", "WRITE");
    try {
      await api.replaceCriticalProcessUsers([...new Set(parsed.data)]);
    } catch (error) {
      // The procedure reports 0 affected rows (404) when the list did not change.
      if (!(error instanceof ApiError && error.status === 404)) throw error;
    }
    revalidatePath("/settings/critical-users");
  });
}
