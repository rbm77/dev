package com.bmsoftware.smsbridge.ui

import androidx.lifecycle.ViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow

/**
 * Estado del desbloqueo por PIN. Vive en el ViewModel: sobrevive a un giro de pantalla,
 * pero no se guarda en el estado restaurable, así que si el proceso muere hay que volver a poner el PIN.
 */
class SessionViewModel : ViewModel() {

    private val _unlocked = MutableStateFlow(false)
    val unlocked: StateFlow<Boolean> = _unlocked.asStateFlow()

    fun unlock() {
        _unlocked.value = true
    }

    fun lock() {
        _unlocked.value = false
    }
}
