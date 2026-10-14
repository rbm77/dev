Observaciones del backend (no lo toqué)
Seguridad:

PUT /users/critical-process no verifica que quien llama sea usuario de proceso crítico. Cualquier usuario con permiso de escritura en usuarios puede volverse crítico.
POST /users/reset-password no requiere sesión y cambia la contraseña de inmediato. Cualquiera puede dejar a otro usuario sin acceso, y la contraseña nueva nunca se envía ni se devuelve.
Las contraseñas se guardan en texto plano.
El login no limita los intentos fallidos.
La clave del JWT y la cadena de conexión están en appsettings.json, dentro del repositorio.
Funcionales:
6. La tabla de recursos no incluye WHATSAPP_ACCOUNT. Por eso ese permiso no se puede asignar desde la pantalla de roles.
7. A match-pending le falta su API key en ServiceApiKeys.
8. Varios listados devuelven 0 en columnas que no consultan (por ejemplo, el monto en pagos). El frontend nunca muestra esos valores.
9. /openapi/v1.json exige autenticación.