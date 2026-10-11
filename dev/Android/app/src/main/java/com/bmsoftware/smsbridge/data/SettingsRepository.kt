package com.bmsoftware.smsbridge.data

import android.content.Context
import androidx.core.content.edit
import com.bmsoftware.smsbridge.domain.AppSettings

/**
 * Configuración y estado de activación en SharedPreferences, en claro.
 * Se cachea en memoria para que el worker y el receptor lean sin esperar a disco.
 */
class SettingsRepository(context: Context) {

    private val prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)

    @Volatile
    private var cached: AppSettings = read()

    @Volatile
    private var cachedEnabled: Boolean = prefs.getBoolean(KEY_ENABLED, false)

    fun load(): AppSettings = cached

    /** Guarda solo la configuración. No cambia el estado de activación. */
    fun save(settings: AppSettings) {
        prefs.edit {
            putString(KEY_URL, settings.url)
            putString(KEY_API_KEY, settings.apiKey)
            putLong(KEY_COMPANY_ID, settings.companyId)
            putInt(KEY_RETRY_MINUTES, settings.retryMinutes)
        }
        cached = settings
    }

    fun isEnabled(): Boolean = cachedEnabled

    fun setEnabled(value: Boolean) {
        prefs.edit { putBoolean(KEY_ENABLED, value) }
        cachedEnabled = value
    }

    private fun read() = AppSettings(
        url = prefs.getString(KEY_URL, "").orEmpty(),
        apiKey = prefs.getString(KEY_API_KEY, "").orEmpty(),
        companyId = prefs.getLong(KEY_COMPANY_ID, 0L),
        retryMinutes = prefs.getInt(KEY_RETRY_MINUTES, AppSettings.DEFAULT_RETRY_MINUTES),
    )

    private companion object {
        const val PREFS_NAME = "settings"
        const val KEY_URL = "url"
        const val KEY_API_KEY = "api_key"
        const val KEY_COMPANY_ID = "company_id"
        const val KEY_RETRY_MINUTES = "retry_minutes"
        const val KEY_ENABLED = "enabled"
    }
}
