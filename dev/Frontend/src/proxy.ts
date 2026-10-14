import { NextResponse, type NextRequest } from "next/server";
import { decodeToken, isExpired } from "@/lib/jwt";
import { SESSION_COOKIE, SESSION_COOKIE_OPTIONS } from "@/lib/session-cookie";

// Optimistic checks only (cookie present and not expired). Real authorization
// happens in the Data Access Layer and in the API on every request.

const PUBLIC_PATHS = ["/login", "/forgot-password", "/session-expired", "/offline"];
const GUEST_ONLY_PATHS = ["/login", "/forgot-password"];

const isDevelopment = process.env.NODE_ENV === "development";

function contentSecurityPolicy(nonce: string): string {
  const directives = [
    "default-src 'self'",
    `script-src 'self' 'nonce-${nonce}' 'strict-dynamic'${isDevelopment ? " 'unsafe-eval'" : ""}`,
    // Radix and Sonner set inline style attributes, which nonces cannot cover.
    "style-src 'self' 'unsafe-inline'",
    "img-src 'self' data: blob:",
    "font-src 'self'",
    "connect-src 'self'",
    "worker-src 'self'",
    "manifest-src 'self'",
    "object-src 'none'",
    "base-uri 'self'",
    "form-action 'self'",
    "frame-ancestors 'none'",
  ];
  // No `upgrade-insecure-requests`: every resource is same-origin and HSTS
  // already enforces HTTPS in production; it would only break local `next start`.
  return directives.join("; ");
}

function isPublicPath(pathname: string): boolean {
  return PUBLIC_PATHS.some((path) => pathname === path || pathname.startsWith(`${path}/`));
}

export function proxy(request: NextRequest) {
  const { pathname, search } = request.nextUrl;
  const token = request.cookies.get(SESSION_COOKIE)?.value;
  const identity = token ? decodeToken(token) : null;
  const hasSession = identity !== null && !isExpired(identity);

  if (!hasSession && !isPublicPath(pathname)) {
    const loginUrl = new URL("/login", request.url);
    if (pathname !== "/") {
      loginUrl.searchParams.set("next", `${pathname}${search}`);
    }
    if (token) {
      loginUrl.searchParams.set("expired", "1");
    }
    const response = NextResponse.redirect(loginUrl);
    if (token) {
      response.cookies.set(SESSION_COOKIE, "", { ...SESSION_COOKIE_OPTIONS, maxAge: 0 });
    }
    return response;
  }

  if (hasSession && GUEST_ONLY_PATHS.includes(pathname)) {
    return NextResponse.redirect(new URL("/", request.url));
  }

  // Fresh nonce per request; Next.js reads it from the request CSP header and
  // applies it to its own scripts while rendering.
  const nonce = btoa(crypto.randomUUID());
  const csp = contentSecurityPolicy(nonce);

  const requestHeaders = new Headers(request.headers);
  requestHeaders.set("x-nonce", nonce);
  requestHeaders.set("Content-Security-Policy", csp);

  const response = NextResponse.next({ request: { headers: requestHeaders } });
  response.headers.set("Content-Security-Policy", csp);
  return response;
}

export const config = {
  matcher: [
    // Everything except API route handlers (they authorize themselves), build
    // assets, app icons, the manifest, the service worker and public images.
    // These must stay reachable without a session (install prompts fetch them).
    "/((?!api|_next/static|_next/image|favicon.ico|icon|apple-icon|pwa-icon|manifest.webmanifest|sw.js|.*\\.(?:png|jpg|jpeg|svg|webp|ico)$).*)",
  ],
};
