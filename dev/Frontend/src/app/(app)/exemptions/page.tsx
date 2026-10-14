import { redirect } from "next/navigation";
import { can } from "@/lib/permissions";
import { verifySession } from "@/lib/session";

export default async function ExemptionsPage() {
  const session = await verifySession();
  redirect(can(session.permissions, "SPECIFIC_EXEMPTION", "READ") ? "/exemptions/specific" : "/exemptions/periodic");
}
