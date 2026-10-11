package com.bmsoftware.smsbridge.work

import androidx.work.Constraints
import androidx.work.ExistingPeriodicWorkPolicy
import androidx.work.ExistingWorkPolicy
import androidx.work.NetworkType
import androidx.work.OneTimeWorkRequest
import androidx.work.OneTimeWorkRequestBuilder
import androidx.work.PeriodicWorkRequestBuilder
import androidx.work.WorkManager
import androidx.work.workDataOf
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import java.util.concurrent.TimeUnit

class SmsWorkScheduler(private val workManager: WorkManager) {

    /** Primer intento, inmediato cuando hay red. */
    fun enqueueFirstAttempt(id: Long) {
        workManager.enqueueUniqueWork(
            firstAttemptName(id),
            ExistingWorkPolicy.KEEP,
            buildRequest(id, SendSmsWorker.FIRST_ATTEMPT, delayMinutes = 0),
        )
    }

    /**
     * Reintento. Usa otro nombre único porque el worker del primer intento
     * sigue en ejecución cuando lo encola. Con KEEP, WorkManager descartaría la petición.
     */
    fun enqueueRetry(id: Long, delayMinutes: Int) {
        workManager.enqueueUniqueWork(
            retryName(id),
            ExistingWorkPolicy.REPLACE,
            buildRequest(id, SendSmsWorker.RETRY_ATTEMPT, delayMinutes),
        )
    }

    /** True si el SMS tiene algún trabajo de envío que todavía no ha terminado. */
    suspend fun hasActiveWork(id: Long): Boolean = withContext(Dispatchers.IO) {
        listOf(firstAttemptName(id), retryName(id)).any { name ->
            workManager.getWorkInfosForUniqueWork(name).get().any { !it.state.isFinished }
        }
    }

    /** Cancela el envío y el reintento de un SMS concreto. */
    fun cancelFor(id: Long) {
        workManager.cancelUniqueWork(firstAttemptName(id))
        workManager.cancelUniqueWork(retryName(id))
    }

    /** Cancela todo lo relacionado con SMS: envíos, reintentos y recuperación periódica. */
    fun cancelAll() {
        workManager.cancelAllWorkByTag(TAG)
    }

    /** Programa la recuperación periódica de filas huérfanas. KEEP la hace idempotente. */
    fun ensureOrphanRecovery() {
        workManager.enqueueUniquePeriodicWork(
            ORPHAN_RECOVERY_NAME,
            ExistingPeriodicWorkPolicy.KEEP,
            PeriodicWorkRequestBuilder<OrphanRecoveryWorker>(RECOVERY_INTERVAL_MINUTES, TimeUnit.MINUTES)
                .addTag(TAG)
                .build(),
        )
    }

    private fun buildRequest(id: Long, attempt: Int, delayMinutes: Int): OneTimeWorkRequest {
        val builder = OneTimeWorkRequestBuilder<SendSmsWorker>()
            .addTag(TAG)
            .setInputData(
                workDataOf(
                    SendSmsWorker.KEY_ID to id,
                    SendSmsWorker.KEY_ATTEMPT to attempt,
                )
            )
            .setConstraints(
                Constraints.Builder()
                    .setRequiredNetworkType(NetworkType.CONNECTED)
                    .build()
            )
        if (delayMinutes > 0) {
            builder.setInitialDelay(delayMinutes.toLong(), TimeUnit.MINUTES)
        }
        return builder.build()
    }

    private fun firstAttemptName(id: Long) = "sms-$id"

    private fun retryName(id: Long) = "sms-$id-retry"

    companion object {
        const val TAG = "sms"
        private const val ORPHAN_RECOVERY_NAME = "orphan-recovery"
        private const val RECOVERY_INTERVAL_MINUTES = 15L
    }
}
