package com.bmsoftware.smsbridge.domain

import java.net.MalformedURLException
import java.net.URL

data class AppSettings(
    val url: String,
    val apiKey: String,
    val companyId: Long,
    val retryMinutes: Int,
) {
    /** True si la configuración permite enviar. Es el mismo criterio que usa la validación de la pantalla. */
    fun isUsable(): Boolean =
        isHttpsUrl(url) && apiKey.isNotBlank() && companyId > 0L && retryMinutes >= 1

    companion object {
        const val DEFAULT_RETRY_MINUTES = 3

        val EMPTY = AppSettings(
            url = "",
            apiKey = "",
            companyId = 0L,
            retryMinutes = DEFAULT_RETRY_MINUTES,
        )

        fun isHttpsUrl(value: String): Boolean = try {
            val parsed = URL(value)
            parsed.protocol == "https" && !parsed.host.isNullOrEmpty()
        } catch (e: MalformedURLException) {
            false
        }
    }
}
