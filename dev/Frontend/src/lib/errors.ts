// User-facing messages for API failures. Raw backend errors are never shown.
export const ERROR_MESSAGES = {
  badRequest: "Revisa los datos ingresados e intenta de nuevo.",
  unauthorized: "Tu sesión expiró. Inicia sesión nuevamente.",
  forbidden:
    "No tienes autorización para esta acción. Algunas operaciones requieren un usuario de proceso crítico.",
  notFound: "El registro no existe o ya fue modificado.",
  server: "Ocurrió un error inesperado. Intenta de nuevo.",
  network: "No se pudo conectar con el servidor. Intenta de nuevo en unos momentos.",
} as const;

export function messageForStatus(status: number): string {
  if (status === 400) return ERROR_MESSAGES.badRequest;
  if (status === 401) return ERROR_MESSAGES.unauthorized;
  if (status === 403) return ERROR_MESSAGES.forbidden;
  if (status === 404) return ERROR_MESSAGES.notFound;
  if (status === 0) return ERROR_MESSAGES.network;
  return ERROR_MESSAGES.server;
}
