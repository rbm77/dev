package com.bmsoftware.smsbridge.ui

import androidx.lifecycle.ViewModel
import com.bmsoftware.smsbridge.ActivationController
import com.bmsoftware.smsbridge.data.SettingsRepository
import com.bmsoftware.smsbridge.domain.ActivationResult
import com.bmsoftware.smsbridge.domain.AppSettings
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow

data class SettingsFormState(
    val url: String = "",
    val apiKey: String = "",
    val companyId: String = "",
    val retryMinutes: String = "",
    val errors: Set<SettingsField> = emptySet(),
    val saved: Boolean = false,
    /** Valor del interruptor en el formulario. Se aplica al guardar. */
    val enabled: Boolean = false,
    /** Estado que está aplicado de verdad en este momento. */
    val appliedEnabled: Boolean = false,
    val enableError: ActivationResult? = null,
)

enum class SettingsField { URL, API_KEY, COMPANY_ID, RETRY_MINUTES }

class SettingsViewModel(
    private val settingsRepository: SettingsRepository,
    private val activation: ActivationController,
) : ViewModel() {

    private val _state = MutableStateFlow(
        settingsRepository.load().toFormState().copy(
            enabled = activation.isEnabled(),
            appliedEnabled = activation.isEnabled(),
        )
    )
    val state: StateFlow<SettingsFormState> = _state.asStateFlow()

    fun onFieldChanged(field: SettingsField, value: String) {
        val current = _state.value
        val updated = when (field) {
            SettingsField.URL -> current.copy(url = value)
            SettingsField.API_KEY -> current.copy(apiKey = value)
            SettingsField.COMPANY_ID -> current.copy(companyId = value)
            SettingsField.RETRY_MINUTES -> current.copy(retryMinutes = value)
        }
        _state.value = updated.copy(errors = current.errors - field, saved = false)
    }

    /** Solo cambia el formulario. El interruptor no tiene efecto hasta que se guarda. */
    fun onEnabledToggled(wanted: Boolean) {
        _state.value = _state.value.copy(enabled = wanted, saved = false, enableError = null)
    }

    /**
     * Guarda la configuración y aplica el interruptor.
     * Activar exige configuración válida y permiso de SMS. Si falla, la app queda desactivada y se indica el motivo.
     */
    fun save() {
        val current = _state.value
        val errors = validate(current)
        if (errors.isNotEmpty()) {
            _state.value = current.copy(errors = errors, saved = false)
            return
        }

        settingsRepository.save(
            AppSettings(
                url = current.url.trim(),
                apiKey = current.apiKey.trim(),
                companyId = current.companyId.trim().toLong(),
                retryMinutes = current.retryMinutes.trim().toInt(),
            )
        )

        if (current.enabled) {
            when (val result = activation.enable()) {
                ActivationResult.Enabled -> _state.value = current.copy(
                    errors = emptySet(),
                    saved = true,
                    appliedEnabled = true,
                    enableError = null,
                )

                else -> {
                    if (activation.isEnabled()) activation.disable()
                    _state.value = current.copy(
                        enabled = false,
                        appliedEnabled = false,
                        errors = emptySet(),
                        saved = false,
                        enableError = result,
                    )
                }
            }
            return
        }

        if (activation.isEnabled()) activation.disable()
        _state.value = current.copy(
            errors = emptySet(),
            saved = true,
            appliedEnabled = false,
            enableError = null,
        )
    }

    /** Se llama cuando el aviso de "guardado" ya se mostró, para que no vuelva a aparecer. */
    fun onSaveMessageShown() {
        _state.value = _state.value.copy(saved = false)
    }

    fun onEnableErrorShown() {
        _state.value = _state.value.copy(enableError = null)
    }

    private fun validate(state: SettingsFormState): Set<SettingsField> = buildSet {
        if (!AppSettings.isHttpsUrl(state.url.trim())) add(SettingsField.URL)
        if (state.apiKey.isBlank()) add(SettingsField.API_KEY)
        val companyId = state.companyId.trim().toLongOrNull()
        if (companyId == null || companyId <= 0L) add(SettingsField.COMPANY_ID)
        val retry = state.retryMinutes.trim().toIntOrNull()
        if (retry == null || retry < 1) add(SettingsField.RETRY_MINUTES)
    }

    private fun AppSettings.toFormState() = SettingsFormState(
        url = url,
        apiKey = apiKey,
        companyId = companyId.toString(),
        retryMinutes = retryMinutes.toString(),
    )
}
