import "server-only";

import type * as z from "zod";
import { apiGetOptional, apiSend } from "@/lib/api";
import type { companySchema } from "./schema";
import type { Company } from "./types";

export function getCompany(id: number) {
  return apiGetOptional<Company>(`/companies/${id}`);
}

/** PUT /companies/{id}: { name, phoneNumber?, email?, isActive, autoApprovalEnabled } */
export function updateCompany(id: number, company: z.output<typeof companySchema> & { isActive: boolean }) {
  return apiSend("PUT", `/companies/${id}`, company);
}
