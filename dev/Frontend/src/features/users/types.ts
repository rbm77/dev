// Features/Users contract.

/** GET /users — the list has no address/phone/email. */
export type UserListItem = {
  id: number;
  username?: string;
  roleId: number;
  isActive: boolean;
  identityDocument?: string;
  name?: string;
  lastName?: string;
};

/** GET /users/{id} and GET /users/{id}/own (never includes the password). */
export type User = UserListItem & { address?: string; phoneNumber?: string; email?: string };

/** GET /users/critical-process */
export type CriticalProcessUser = { id: number; name?: string; lastName?: string };
