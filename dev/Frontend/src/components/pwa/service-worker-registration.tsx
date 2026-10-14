"use client";

import { useEffect } from "react";

/**
 * Registers /sw.js in production. In development any previous registration is
 * removed so cached build assets never interfere with hot reloading.
 */
export function ServiceWorkerRegistration() {
  useEffect(() => {
    if (!("serviceWorker" in navigator)) {
      return;
    }

    if (process.env.NODE_ENV !== "production") {
      void navigator.serviceWorker
        .getRegistrations()
        .then((registrations) => Promise.all(registrations.map((registration) => registration.unregister())));
      return;
    }

    navigator.serviceWorker.register("/sw.js", { scope: "/", updateViaCache: "none" }).catch((error: unknown) => {
      console.error("No se pudo registrar el service worker:", error);
    });
  }, []);

  return null;
}
