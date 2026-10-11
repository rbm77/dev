package com.bmsoftware.smsbridge.domain

/** Resultado de intentar activar la app. */
sealed interface ActivationResult {
    data object Enabled : ActivationResult

    /** Falta URL https, API Key, companyId o minutos de reintento válidos. */
    data object InvalidSettings : ActivationResult

    /** Falta el permiso para recibir SMS. */
    data object MissingPermission : ActivationResult
}
