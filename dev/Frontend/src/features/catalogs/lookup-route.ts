import "server-only";

import { NextResponse, type NextRequest } from "next/server";
import { unstable_rethrow } from "next/navigation";
import type { ComboboxItem } from "@/components/shared/entity-combobox";
import { ApiError } from "@/lib/api";
import { can, type Resource } from "@/lib/permissions";
import { getSession } from "@/lib/session";

const MAX_QUERY = 50;

function json(items: ComboboxItem[], status = 200) {
  // Results contain personal data: never cache them anywhere.
  return NextResponse.json({ items }, { status, headers: { "Cache-Control": "no-store" } });
}

/** Shared GET handler for the search endpoints used by EntityCombobox. */
export async function handleLookup(
  request: NextRequest,
  resource: Resource,
  search: (term: string) => Promise<ComboboxItem[]>,
): Promise<NextResponse> {
  const session = await getSession();
  if (!session) {
    return json([], 401);
  }
  if (!can(session.permissions, resource, "READ")) {
    return json([], 403);
  }

  const term = (request.nextUrl.searchParams.get("q") ?? "").trim().slice(0, MAX_QUERY);
  if (term.length < 2) {
    return json([]);
  }

  try {
    return json(await search(term));
  } catch (error) {
    unstable_rethrow(error);
    return json([], error instanceof ApiError && error.status > 0 ? error.status : 502);
  }
}
