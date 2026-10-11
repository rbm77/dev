package com.bmsoftware.smsbridge.receiver

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.provider.Telephony
import com.bmsoftware.smsbridge.SmsBridgeApp
import kotlinx.coroutines.launch

/** Solo guarda el SMS y encola el envío. Nunca envía dentro del receiver. */
class SmsReceiver : BroadcastReceiver() {

    override fun onReceive(context: Context, intent: Intent) {
        if (intent.action != Telephony.Sms.Intents.SMS_RECEIVED_ACTION) return

        val app = context.applicationContext as SmsBridgeApp
        // Con la app desactivada, el SMS se ignora por completo.
        if (!app.container.settingsRepository.isEnabled()) return

        // Un SMS largo llega en varias partes; se concatenan en el orden recibido.
        val text = Telephony.Sms.Intents.getMessagesFromIntent(intent)
            ?.joinToString(separator = "") { it.messageBody.orEmpty() }
            ?: return
        if (text.isEmpty()) return

        val pendingResult = goAsync()

        app.appScope.launch {
            try {
                val id = app.container.pendingSmsRepository.save(text)
                app.container.workScheduler.enqueueFirstAttempt(id)
            } catch (e: Exception) {
                // Sin logs. Si la fila quedó sin trabajo, la tarea periódica la recupera.
            } finally {
                pendingResult.finish()
            }
        }
    }
}
