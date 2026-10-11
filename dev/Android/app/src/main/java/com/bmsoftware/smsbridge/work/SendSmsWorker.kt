package com.bmsoftware.smsbridge.work

import android.content.Context
import androidx.work.CoroutineWorker
import androidx.work.WorkerParameters
import com.bmsoftware.smsbridge.SmsBridgeApp
import com.bmsoftware.smsbridge.domain.SendResult
import kotlinx.coroutines.CancellationException

/**
 * Envía un SMS pendiente. Sirve para el primer intento y para el reintento.
 * El número de intento viaja en los datos del trabajo, no solo en la base de datos.
 */
class SendSmsWorker(context: Context, params: WorkerParameters) : CoroutineWorker(context, params) {

    override suspend fun doWork(): Result {
        val id = inputData.getLong(KEY_ID, -1L)
        if (id < 0) return Result.success()
        val attempt = inputData.getInt(KEY_ATTEMPT, FIRST_ATTEMPT)

        val container = (applicationContext as SmsBridgeApp).container
        // Con la app desactivada no se llama a la API, aunque el trabajo siga en cola.
        if (!container.settingsRepository.isEnabled()) return Result.success()
        val repository = container.pendingSmsRepository
        val sms = repository.get(id) ?: return Result.success()
        val settings = container.settingsRepository.load()

        val result = try {
            container.smsApiClient.send(settings, sms.text)
        } catch (e: CancellationException) {
            throw e
        } catch (e: Exception) {
            // Cualquier fallo inesperado, por ejemplo una configuración corrupta, cuenta como rechazo.
            SendResult.Rejected
        }

        return when (result) {
            SendResult.Accepted -> {
                repository.delete(id)
                Result.success()
            }

            // Sin conexión: el mensaje queda intacto y no cuenta como intento.
            SendResult.NoConnection -> Result.retry()

            SendResult.Rejected -> {
                if (attempt == FIRST_ATTEMPT) {
                    // Primero se programa el reintento y después se marca la fila.
                    // Si el proceso muere entre las dos cosas, el reintento ya existe.
                    container.workScheduler.enqueueRetry(id, settings.retryMinutes)
                    repository.markRetry(id)
                } else {
                    // Segundo fallo: se descarta.
                    repository.delete(id)
                }
                Result.success()
            }
        }
    }

    companion object {
        const val KEY_ID = "sms_id"
        const val KEY_ATTEMPT = "attempt"
        const val FIRST_ATTEMPT = 0
        const val RETRY_ATTEMPT = 1
    }
}
