import "server-only";

import type { ComboboxItem } from "@/components/shared/entity-combobox";
import { apiGet, type Paged } from "@/lib/api";
import { LIMITS } from "@/lib/db-limits";
import { fullName } from "@/lib/format";
import type { StudentListItem } from "./types";

const SEARCH_LIMIT = 10;

type NamedRow = { id: number; name?: string; lastName?: string; identityDocument?: string };

function looksLikeIdentity(term: string): boolean {
  return /\d/.test(term) && !/\s/.test(term) && term.length <= LIMITS.personalData.identityDocument;
}

/**
 * The API filters name/lastName with "contains" and identityDocument with an
 * exact match, so the free text is mapped to the most useful combination.
 */
async function searchByName<T extends NamedRow>(path: string, term: string): Promise<T[]> {
  const base = { page: 1, pageSize: SEARCH_LIMIT };

  if (looksLikeIdentity(term)) {
    return (await apiGet<Paged<T>>(path, { ...base, identityDocument: term })).items;
  }

  const [first, ...rest] = term.split(/\s+/);
  if (rest.length > 0) {
    return (await apiGet<Paged<T>>(path, { ...base, name: first, lastName: rest.join(" ") })).items;
  }

  const [byName, byLastName] = await Promise.all([
    apiGet<Paged<T>>(path, { ...base, name: term }),
    apiGet<Paged<T>>(path, { ...base, lastName: term }),
  ]);
  const unique = new Map<number, T>();
  for (const row of [...byName.items, ...byLastName.items]) {
    unique.set(row.id, row);
  }
  return [...unique.values()].slice(0, SEARCH_LIMIT);
}

export async function searchStudents(term: string): Promise<ComboboxItem[]> {
  const students = await searchByName<StudentListItem>("/students", term);
  return students.map((student) => ({
    value: String(student.id),
    label: fullName(student.name, student.lastName),
    detail: [student.identityDocument, student.isActive ? null : "Inactivo"].filter(Boolean).join(" · ") || undefined,
  }));
}

export async function searchUsers(term: string): Promise<ComboboxItem[]> {
  const users = await searchByName<NamedRow & { username?: string }>("/users", term);
  return users.map((user) => ({
    value: String(user.id),
    label: fullName(user.name, user.lastName),
    detail: user.username,
  }));
}
