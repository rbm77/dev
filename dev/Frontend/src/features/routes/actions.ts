"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { authorize } from "@/lib/authorize";
import { parseId } from "@/lib/validation";
import * as api from "./api";
import { routeSchema } from "./schema";

const INVALID_ID: ActionResult<never> = { ok: false, error: "Registro no válido." };

export async function createRoute(input: unknown): Promise<ActionResult> {
  const parsed = routeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("ROUTE", "WRITE");
    await api.createRoute(parsed.data);
    revalidatePath("/routes");
  });
}

export async function updateRoute(id: unknown, input: unknown): Promise<ActionResult> {
  const routeId = parseId(id);
  if (!routeId) return INVALID_ID;
  const parsed = routeSchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    await authorize("ROUTE", "WRITE");
    await api.updateRoute(routeId, parsed.data);
    revalidatePath("/routes");
  });
}

export async function deleteRoute(id: unknown): Promise<ActionResult> {
  const routeId = parseId(id);
  if (!routeId) return INVALID_ID;

  return runAction(async () => {
    await authorize("ROUTE", "WRITE");
    await api.deleteRoute(routeId);
    revalidatePath("/routes");
  });
}
