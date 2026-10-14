/*
 * Buslogix service worker.
 *
 * Security: only public, content-hashed build assets, the app icons and the
 * /offline page are cached. HTML pages, RSC payloads, Server Actions and any
 * response with user data always go to the network and are never stored.
 *
 * Bump CACHE_VERSION when this file's caching logic changes.
 */
const CACHE_VERSION = "v2";
const STATIC_CACHE = `buslogix-static-${CACHE_VERSION}`;
const OFFLINE_URL = "/offline";
const PRECACHE_URLS = [OFFLINE_URL, "/icon", "/apple-icon", "/pwa-icon/192"];
const MAX_STATIC_ENTRIES = 200;

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches
      .open(STATIC_CACHE)
      .then((cache) => cache.addAll(PRECACHE_URLS.map((url) => new Request(url, { cache: "reload" }))))
      .then(() => self.skipWaiting()),
  );
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) =>
        Promise.all(
          keys.filter((key) => key.startsWith("buslogix-") && key !== STATIC_CACHE).map((key) => caches.delete(key)),
        ),
      )
      .then(() => self.clients.claim()),
  );
});

// Next.js links metadata icons with a cache-busting query (/icon?abc123).
function isIcon(url) {
  return url.pathname.startsWith("/pwa-icon/") || url.pathname === "/icon" || url.pathname === "/apple-icon";
}

function isCacheableAsset(url) {
  return url.pathname.startsWith("/_next/static/") || isIcon(url);
}

async function trimCache(cache) {
  const keys = await cache.keys();
  if (keys.length > MAX_STATIC_ENTRIES) {
    await Promise.all(keys.slice(0, keys.length - MAX_STATIC_ENTRIES).map((key) => cache.delete(key)));
  }
}

async function cacheFirst(request, ignoreSearch) {
  const cache = await caches.open(STATIC_CACHE);
  const cached = await cache.match(request, { ignoreSearch });
  if (cached) {
    return cached;
  }
  try {
    const response = await fetch(request);
    if (response.ok && response.type === "basic") {
      await cache.put(request, response.clone());
      await trimCache(cache);
    }
    return response;
  } catch {
    return Response.error();
  }
}

async function networkWithOfflineFallback(request) {
  try {
    return await fetch(request);
  } catch {
    const offline = await caches.match(OFFLINE_URL);
    return offline ?? Response.error();
  }
}

self.addEventListener("fetch", (event) => {
  const { request } = event;
  if (request.method !== "GET") {
    return;
  }

  const url = new URL(request.url);
  if (url.origin !== self.location.origin) {
    return;
  }

  if (request.mode === "navigate") {
    event.respondWith(networkWithOfflineFallback(request));
    return;
  }

  if (isCacheableAsset(url)) {
    event.respondWith(cacheFirst(request, isIcon(url)));
  }
});
