"use server";

import { revalidatePath } from "next/cache";
import type { ActionResult } from "@/lib/action-result";
import { invalidInput, runAction } from "@/lib/actions";
import { ApiError } from "@/lib/api";
import { authorize } from "@/lib/authorize";
import { messageForStatus } from "@/lib/errors";
import { getCompany, updateCompany } from "./api";
import { companySchema } from "./schema";

/** The company id comes from the session (cid claim); isActive is re-read, never taken from the client. */
export async function saveCompany(input: unknown): Promise<ActionResult> {
  const parsed = companySchema.safeParse(input);
  if (!parsed.success) return invalidInput(parsed.error);

  return runAction(async () => {
    const session = await authorize("COMPANY", "WRITE");
    const current = await getCompany(session.companyId);
    if (!current) {
      throw new ApiError(404, messageForStatus(404));
    }
    await updateCompany(session.companyId, { ...parsed.data, isActive: current.isActive });
    revalidatePath("/settings/company");
  });
}
