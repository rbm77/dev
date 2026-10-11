package com.bmsoftware.smsbridge

import android.Manifest
import android.content.ComponentName
import android.content.Context
import android.content.pm.PackageManager
import androidx.core.content.ContextCompat
import com.bmsoftware.smsbridge.data.PendingSmsRepository
import com.bmsoftware.smsbridge.data.SettingsRepository
import com.bmsoftware.smsbridge.domain.ActivationResult
import com.bmsoftware.smsbridge.receiver.SmsReceiver
import com.bmsoftware.smsbridge.work.SmsWorkScheduler
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch

/**
 * Único punto que cambia el estado de activación.
 * Desactivar deja la app sin recibir, guardar ni enviar nada.
 */
class ActivationController(
    private val context: Context,
    private val settingsRepository: SettingsRepository,
    private val pendingSmsRepository: PendingSmsRepository,
    private val scheduler: SmsWorkScheduler,
    private val appScope: CoroutineScope,
) {
    private val receiver = ComponentName(context, SmsReceiver::class.java)

    fun isEnabled(): Boolean = settingsRepository.isEnabled()

    fun enable(): ActivationResult {
        if (!settingsRepository.load().isUsable()) return ActivationResult.InvalidSettings
        if (!hasSmsPermission()) return ActivationResult.MissingPermission

        settingsRepository.setEnabled(true)
        setReceiverEnabled(true)
        scheduler.ensureOrphanRecovery()
        return ActivationResult.Enabled
    }

    fun disable() {
        // Primero se apaga el flag, para que los workers que aún corran no envíen nada.
        settingsRepository.setEnabled(false)
        scheduler.cancelAll()
        // Android deja de despertar la app para los SMS.
        setReceiverEnabled(false)
        appScope.launch { pendingSmsRepository.deleteAll() }
    }

    /** Alinea el receptor y la recuperación con el estado guardado. Se llama al nacer el proceso. */
    fun syncOnStart() {
        if (settingsRepository.isEnabled()) {
            setReceiverEnabled(true)
            scheduler.ensureOrphanRecovery()
        } else {
            setReceiverEnabled(false)
            scheduler.cancelAll()
        }
    }

    private fun setReceiverEnabled(enabled: Boolean) {
        val desired = if (enabled) {
            PackageManager.COMPONENT_ENABLED_STATE_ENABLED
        } else {
            PackageManager.COMPONENT_ENABLED_STATE_DISABLED
        }
        val packageManager = context.packageManager
        // Solo se escribe si cambia, para no tocar PackageManager en cada arranque.
        if (packageManager.getComponentEnabledSetting(receiver) != desired) {
            packageManager.setComponentEnabledSetting(receiver, desired, PackageManager.DONT_KILL_APP)
        }
    }

    private fun hasSmsPermission(): Boolean =
        ContextCompat.checkSelfPermission(context, Manifest.permission.RECEIVE_SMS) ==
            PackageManager.PERMISSION_GRANTED
}
