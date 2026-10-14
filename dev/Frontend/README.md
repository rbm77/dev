# Buslogix Web

Frontend (PWA) de Buslogix. Usa Next.js (App Router), React, TypeScript, Tailwind CSS y shadcn/ui.
Es el único consumidor de la API `dev/Backend/Buslogix`: el navegador nunca llama a la API directamente.
El servidor de Next actúa como BFF y guarda el JWT en una cookie `httpOnly`.

## Requisitos

- Node.js LTS (20.9 o superior).
- La API de Buslogix en ejecución. En desarrollo se usa el perfil `http`:
  `dotnet run --launch-profile http` en `dev/Backend/Buslogix/Buslogix`, que escucha en el puerto 5192.

## Configuración

Crea un archivo `.env.local`. No lo subas al repositorio.

```
API_BASE_URL=http://localhost:5192
```

`API_BASE_URL` solo se lee en el servidor. En producción debe ser una URL HTTPS.

## Scripts

| Comando | Uso |
|---|---|
| `npm run dev` | Desarrollo en http://localhost:3000 |
| `npm run lint` | ESLint |
| `npm run build` | Compilación de producción, que incluye el chequeo de tipos |
| `npm start` | Servidor de producción (después de `build`) |

En producción la aplicación debe servirse por **HTTPS**:
- la cookie de sesión usa el prefijo `__Host-` y es `Secure`;
- el service worker solo funciona en un contexto seguro.

## Estructura

```
src/
  proxy.ts         chequeo optimista de sesión + CSP con nonce
  app/             rutas (App Router): (auth) login, (app) módulos, manifest e íconos de la PWA
  components/      ui (shadcn), layout, shared, forms
  features/<x>/    types · schema (zod) · api (server-only) · actions (Server Actions) · components
  lib/             cliente de la API, sesión, permisos, validación, límites de la BD, formato
public/sw.js       service worker: solo cachea assets estáticos y la página /offline
```

## Convenciones

- **Lecturas**: Server Components que llaman a `features/<x>/api.ts`.
- **Escrituras**: Server Actions que hacen tres cosas:
  1. vuelven a validar la entrada con el mismo esquema zod del formulario;
  2. verifican el permiso;
  3. llaman a la API.
- **Límites de los campos**: salen de las tablas de la base de datos (`src/lib/db-limits.ts`).
- **Filtros, pestañas y paginación**: viven en la URL.
- **Permisos**: la UI oculta lo que el usuario no puede hacer, pero la autorización real siempre la aplica la API.
