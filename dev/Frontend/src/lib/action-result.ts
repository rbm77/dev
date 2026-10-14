/** Value returned by every Server Action to the client. Never contains raw backend data. */
export type ActionResult<T = void> =
  | { ok: true; data: T }
  | { ok: false; error: string; fieldErrors?: Partial<Record<string, string[]>> };
