package com.bmsoftware.smsbridge.domain

sealed interface SendResult {
    /** La API respondió 202. */
    data object Accepted : SendResult

    /** Cualquier otra respuesta, o un fallo de red con internet disponible. */
    data object Rejected : SendResult

    /** No hay conexión. El intento no cuenta. */
    data object NoConnection : SendResult
}
