package com.bmsoftware.smsbridge.work

import android.content.Context
import androidx.work.CoroutineWorker
import androidx.work.WorkerParameters
import com.bmsoftware.smsbridge.SmsBridgeApp
import java.util.concurrent.TimeUnit

/**
 * Tarea periódica. Busca filas de SMS que se quedaron sin trabajo de envío
 * (por ejemplo, si el proceso murió entre guardar la fila y encolar) y las reencola.
 * Las filas más antiguas que MAX_AGE_MS se borran sin enviarse.
 */
class OrphanRecoveryWorker(context: Context, params: WorkerParameters) : CoroutineWorker(context, params) {

    override suspend fun doWork(): Result {
        val container = (applicationContext as SmsBridgeApp).container
        if (!container.settingsRepository.isEnabled()) return Result.success()
        val repository = container.pendingSmsRepository
        val scheduler = container.workScheduler

        repository.deleteOlderThan(System.currentTimeMillis() - MAX_AGE_MS)

        for (sms in repository.getAll()) {
            // Si ya hay un envío o reintento en curso, no se toca. Así no se duplican envíos.
            if (scheduler.hasActiveWork(sms.id)) continue

            if (sms.attempts == SendSmsWorker.FIRST_ATTEMPT) {
                scheduler.enqueueFirstAttempt(sms.id)
            } else {
                scheduler.enqueueRetry(sms.id, delayMinutes = 0)
            }
        }
        return Result.success()
    }

    private companion object {
        val MAX_AGE_MS = TimeUnit.HOURS.toMillis(48)
    }
}
