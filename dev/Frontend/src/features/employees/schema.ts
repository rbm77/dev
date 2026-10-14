import * as z from "zod";
import { isIsoDate } from "@/lib/dates";
import { LIMITS } from "@/lib/db-limits";
import { personalDataShape } from "@/lib/schemas";
import { isoDate, MESSAGES, money } from "@/lib/validation";

/**
 * POST /employees, PUT /employees/{id}. The form uses a flat "isDriver" switch;
 * the output becomes the API shape `driverDetails: { licenseNumber, licenseExpiryDate } | null`.
 */
export const employeeSchema = z
  .object({
    ...personalDataShape,
    hireDate: isoDate(),
    isActive: z.boolean(),
    isDriver: z.boolean(),
    licenseNumber: z.string().trim().max(LIMITS.driver.licenseNumber, MESSAGES.maxChars(LIMITS.driver.licenseNumber)),
    licenseExpiryDate: z.string(),
  })
  .superRefine((data, ctx) => {
    if (!data.isDriver) {
      return;
    }
    if (!data.licenseNumber) {
      ctx.addIssue({ code: "custom", path: ["licenseNumber"], message: MESSAGES.required });
    }
    if (!isIsoDate(data.licenseExpiryDate)) {
      ctx.addIssue({
        code: "custom",
        path: ["licenseExpiryDate"],
        message: data.licenseExpiryDate ? MESSAGES.date : MESSAGES.required,
      });
    }
  })
  .transform(({ isDriver, licenseNumber, licenseExpiryDate, ...employee }) => ({
    ...employee,
    driverDetails: isDriver ? { licenseNumber, licenseExpiryDate } : null,
  }));

export type EmployeeFormValues = z.input<typeof employeeSchema>;

/** POST /employees/{id}/salaries — only the amount; the start date is set by the database. */
export const salarySchema = z.object({ amount: money() });

export type SalaryFormValues = z.input<typeof salarySchema>;
