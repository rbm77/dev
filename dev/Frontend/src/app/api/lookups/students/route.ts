import type { NextRequest } from "next/server";
import { handleLookup } from "@/features/catalogs/lookup-route";
import { searchStudents } from "@/features/catalogs/search";

export function GET(request: NextRequest) {
  return handleLookup(request, "STUDENT", searchStudents);
}
