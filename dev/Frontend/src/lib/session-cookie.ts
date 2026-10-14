const isProduction = process.env.NODE_ENV === "production";

// The `__Host-` prefix makes the browser reject the cookie unless it is Secure,
// has Path=/ and no Domain. It requires HTTPS, so it is only used in production.
export const SESSION_COOKIE = isProduction ? "__Host-buslogix-session" : "buslogix-session";

export const SESSION_COOKIE_OPTIONS = {
  httpOnly: true,
  secure: isProduction,
  sameSite: "lax",
  path: "/",
} as const;
