import type { NextRequest } from "next/server";
import { handleLookup } from "@/features/catalogs/lookup-route";
import { searchUsers } from "@/features/catalogs/search";

export function GET(request: NextRequest) {
  return handleLookup(request, "USER", searchUsers);
}
