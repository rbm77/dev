# Notas de diseño de los background workers

Este documento preserva explicaciones de diseño que antes vivían como comentarios en el código
y que no son evidentes solo leyendo la lógica. Se extrajo antes de eliminar los comentarios del
proyecto.

## Orden de bloqueo en el matching de pagos (`PaymentMatchWorker` / `match_payment_request_extraction`)

`match_payment_request_extraction` siempre bloquea (`FOR UPDATE`) primero la fila de
`payment_request` y después la fila de `message_extraction_result`, en ese orden fijo. Esto es
lo que evita que dos ejecuciones concurrentes de este mismo procedimiento se hagan deadlock entre
sí. `PaymentMatchWorker` corre 2 consumidores concurrentes sobre `PaymentMatchQueue` — no porque
el matching sea lento (es un round-trip rápido a MySQL), sino como colchón barato ante ráfagas:
un consumidor ocioso no cuesta nada (el scope de DI y la conexión solo se piden al sacar un item
de la cola), y la corrección nunca depende del número de consumidores — la garantiza el orden
fijo de locks descrito arriba.

## Por qué un reintento de extracción que vuelve a fallar no se reinserta

Cuando `MessageExtractionRetryQueue` dispara un reintento, los items reencolados llevan
`IngestionSource.Retry`. Si la extracción de uno de esos items vuelve a fallar,
`MessageExtractionWorker` lo loguea pero **no** lo reinserta en `message_extraction_failure`
(a diferencia de un fallo de primera vez, que sí se preserva ahí). La razón: ese item ya tuvo su
segunda oportunidad — reinsertarlo crearía un ciclo infinito de reintento → falla → reintento en
cada corrida futura del trigger de reintentos.

## Qué significa un fallo de facturación del LLM en `ClaudeChatCompletionClient`

Un error específico devuelto por la API de Anthropic (crédito prepago agotado) se loguea como
`Error` en vez de `Warning` a propósito: no es un problema transitorio de red ni un mensaje mal
formado, es una cuenta sin fondos — requiere que una persona recargue el crédito prepago de la
cuenta de Anthropic para que el fallback de Tier 2 vuelva a funcionar.

## Por qué el log dentro de `PatternProvider.Compile` es fire-and-forget

`Compile` se invoca tanto desde el constructor de `PatternProvider` como desde el callback
síncrono de `OptionsMonitor.OnChange` (cuando `extraction-patterns.json` cambia en disco).
Ninguno de esos dos contextos puede ser `async`, así que la llamada a `WriteLog` dentro de
`Compile` se descarta intencionalmente (`_ = logHandler.WriteLog(...)`) en vez de esperarse — no
es un descuido, es la única opción disponible en un método síncrono llamado desde un constructor
y desde un callback síncrono.

## Por qué la ventana de tiempo del poll de correo es solo una optimización

`MailKitEmailClient` filtra los correos candidatos con una ventana de tiempo
(`account.LastCheckedAt` menos una hora de margen). Esa ventana existe únicamente por
rendimiento — evita escanear el buzón completo en cada poll. La corrección real de "no procesar
el mismo correo dos veces" viene enteramente de la etiqueta `Buslogix/Processed`: un correo
etiquetado nunca vuelve a aparecer en una búsqueda futura, sin importar la ventana de tiempo. Si
la ventana se acortara o alargara, el peor caso sería reprocesar o tardar en ver un correo — nunca
duplicar un correo ya etiquetado.
