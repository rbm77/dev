package com.bmsoftware.smsbridge.ui

import android.Manifest
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.PowerManager
import android.provider.Settings
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ColumnScope
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.imePadding
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SnackbarHost
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Switch
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.input.ImeAction
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.unit.dp
import androidx.core.content.ContextCompat
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.compose.LifecycleEventEffect
import com.bmsoftware.smsbridge.R
import com.bmsoftware.smsbridge.domain.ActivationResult

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SettingsScreen(
    viewModel: SettingsViewModel,
    onOpenMessages: () -> Unit,
    modifier: Modifier = Modifier,
) {
    val state by viewModel.state.collectAsState()
    val context = LocalContext.current
    val snackbarHostState = remember { SnackbarHostState() }
    val savedMessage = stringResource(R.string.settings_saved)
    val invalidSettingsMessage = stringResource(R.string.enable_error_settings)
    val missingPermissionMessage = stringResource(R.string.enable_error_permission)

    var hasSmsPermission by remember { mutableStateOf(isSmsPermissionGranted(context)) }
    var ignoringBattery by remember { mutableStateOf(isIgnoringBatteryOptimizations(context)) }
    var showApiKey by rememberSaveable { mutableStateOf(false) }

    val smsPermissionLauncher = rememberLauncherForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { granted -> hasSmsPermission = granted }

    val batteryLauncher = rememberLauncherForActivityResult(
        ActivityResultContracts.StartActivityForResult()
    ) { ignoringBattery = isIgnoringBatteryOptimizations(context) }

    // Al volver de los ajustes del sistema se releen los estados.
    LifecycleEventEffect(Lifecycle.Event.ON_RESUME) {
        hasSmsPermission = isSmsPermissionGranted(context)
        ignoringBattery = isIgnoringBatteryOptimizations(context)
    }

    LaunchedEffect(state.saved) {
        if (state.saved) {
            snackbarHostState.showSnackbar(savedMessage)
            viewModel.onSaveMessageShown()
        }
    }

    LaunchedEffect(state.enableError) {
        val error = state.enableError
        if (error != null) {
            val message = when (error) {
                ActivationResult.InvalidSettings -> invalidSettingsMessage
                ActivationResult.MissingPermission -> missingPermissionMessage
                ActivationResult.Enabled -> null
            }
            if (message != null) snackbarHostState.showSnackbar(message)
            viewModel.onEnableErrorShown()
        }
    }

    Scaffold(
        modifier = modifier.fillMaxSize(),
        topBar = { TopAppBar(title = { Text(stringResource(R.string.settings_title)) }) },
        snackbarHost = { SnackbarHost(snackbarHostState) },
    ) { innerPadding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(innerPadding)
                // Evita que el teclado tape los campos o el botón Guardar.
                .imePadding()
                .verticalScroll(rememberScrollState())
                .padding(horizontal = 16.dp, vertical = 8.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp),
        ) {
            SectionCard(title = stringResource(R.string.section_status)) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically,
                ) {
                    Column(
                        modifier = Modifier.weight(1f),
                        verticalArrangement = Arrangement.spacedBy(4.dp),
                    ) {
                        Text(
                            text = stringResource(R.string.toggle_receive_send),
                            style = MaterialTheme.typography.bodyLarge,
                        )
                        Text(
                            text = if (state.enabled) {
                                stringResource(R.string.status_active)
                            } else {
                                stringResource(R.string.status_inactive)
                            },
                            style = MaterialTheme.typography.bodyMedium,
                            color = if (state.enabled) {
                                MaterialTheme.colorScheme.primary
                            } else {
                                MaterialTheme.colorScheme.onSurfaceVariant
                            },
                        )
                    }
                    Switch(
                        checked = state.enabled,
                        onCheckedChange = viewModel::onEnabledToggled,
                    )
                }
                if (state.enabled != state.appliedEnabled) {
                    Text(
                        text = stringResource(R.string.toggle_pending_hint),
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.tertiary,
                    )
                }
            }

            SectionCard(title = stringResource(R.string.section_connection)) {
                FormField(
                    label = stringResource(R.string.field_url),
                    value = state.url,
                    onValueChange = { viewModel.onFieldChanged(SettingsField.URL, it) },
                    errorText = if (SettingsField.URL in state.errors) {
                        stringResource(R.string.error_url)
                    } else {
                        null
                    },
                    keyboardOptions = KeyboardOptions(
                        keyboardType = KeyboardType.Uri,
                        imeAction = ImeAction.Next,
                    ),
                )

                FormField(
                    label = stringResource(R.string.field_api_key),
                    value = state.apiKey,
                    onValueChange = { viewModel.onFieldChanged(SettingsField.API_KEY, it) },
                    errorText = if (SettingsField.API_KEY in state.errors) {
                        stringResource(R.string.error_api_key)
                    } else {
                        null
                    },
                    keyboardOptions = KeyboardOptions(imeAction = ImeAction.Next),
                    visualTransformation = if (showApiKey) {
                        VisualTransformation.None
                    } else {
                        PasswordVisualTransformation()
                    },
                    trailingIcon = {
                        TextButton(onClick = { showApiKey = !showApiKey }) {
                            Text(
                                text = stringResource(
                                    if (showApiKey) R.string.hide_api_key else R.string.show_api_key
                                )
                            )
                        }
                    },
                )

                FormField(
                    label = stringResource(R.string.field_company_id),
                    value = state.companyId,
                    onValueChange = { viewModel.onFieldChanged(SettingsField.COMPANY_ID, it) },
                    errorText = if (SettingsField.COMPANY_ID in state.errors) {
                        stringResource(R.string.error_company_id)
                    } else {
                        null
                    },
                    keyboardOptions = KeyboardOptions(
                        keyboardType = KeyboardType.Number,
                        imeAction = ImeAction.Next,
                    ),
                )

                FormField(
                    label = stringResource(R.string.field_retry_minutes),
                    value = state.retryMinutes,
                    onValueChange = { viewModel.onFieldChanged(SettingsField.RETRY_MINUTES, it) },
                    errorText = if (SettingsField.RETRY_MINUTES in state.errors) {
                        stringResource(R.string.error_retry_minutes)
                    } else {
                        null
                    },
                    keyboardOptions = KeyboardOptions(
                        keyboardType = KeyboardType.Number,
                        imeAction = ImeAction.Done,
                    ),
                )
            }

            SectionCard(title = stringResource(R.string.section_permissions)) {
                StatusLine(
                    title = stringResource(R.string.sms_permission_title),
                    ok = hasSmsPermission,
                    okText = stringResource(R.string.status_granted),
                    missingText = stringResource(R.string.status_missing),
                )
                if (!hasSmsPermission) {
                    OutlinedButton(
                        onClick = { smsPermissionLauncher.launch(Manifest.permission.RECEIVE_SMS) },
                        modifier = Modifier.fillMaxWidth(),
                    ) {
                        Text(stringResource(R.string.action_grant_sms))
                    }
                }
            }

            SectionCard(title = stringResource(R.string.section_battery)) {
                StatusLine(
                    title = stringResource(R.string.battery_title),
                    ok = ignoringBattery,
                    okText = stringResource(R.string.battery_excluded),
                    missingText = stringResource(R.string.battery_not_excluded),
                )
                if (!ignoringBattery) {
                    OutlinedButton(
                        onClick = {
                            val intent = Intent(
                                Settings.ACTION_REQUEST_IGNORE_BATTERY_OPTIMIZATIONS,
                                Uri.parse("package:${context.packageName}"),
                            )
                            batteryLauncher.launch(intent)
                        },
                        modifier = Modifier.fillMaxWidth(),
                    ) {
                        Text(stringResource(R.string.action_exclude_battery))
                    }
                }
            }

            Button(
                onClick = viewModel::save,
                modifier = Modifier.fillMaxWidth(),
            ) {
                Text(stringResource(R.string.action_save))
            }

            OutlinedButton(
                onClick = onOpenMessages,
                modifier = Modifier.fillMaxWidth(),
            ) {
                Text(stringResource(R.string.action_view_messages))
            }

            Spacer(Modifier.height(8.dp))
        }
    }
}

@Composable
private fun SectionCard(title: String, content: @Composable ColumnScope.() -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surfaceContainerLow,
        ),
    ) {
        Column(
            modifier = Modifier.padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            Text(
                text = title,
                style = MaterialTheme.typography.titleMedium,
                color = MaterialTheme.colorScheme.primary,
            )
            content()
        }
    }
}

@Composable
private fun FormField(
    label: String,
    value: String,
    onValueChange: (String) -> Unit,
    errorText: String?,
    keyboardOptions: KeyboardOptions,
    visualTransformation: VisualTransformation = VisualTransformation.None,
    trailingIcon: (@Composable () -> Unit)? = null,
) {
    OutlinedTextField(
        value = value,
        onValueChange = onValueChange,
        label = { Text(label) },
        isError = errorText != null,
        supportingText = if (errorText != null) {
            { Text(errorText) }
        } else {
            null
        },
        singleLine = true,
        keyboardOptions = keyboardOptions,
        visualTransformation = visualTransformation,
        trailingIcon = trailingIcon,
        modifier = Modifier.fillMaxWidth(),
    )
}

@Composable
private fun StatusLine(title: String, ok: Boolean, okText: String, missingText: String) {
    Column(verticalArrangement = Arrangement.spacedBy(4.dp)) {
        Text(text = title, style = MaterialTheme.typography.bodyLarge)
        Text(
            text = if (ok) okText else missingText,
            style = MaterialTheme.typography.bodyMedium,
            color = if (ok) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.error,
        )
    }
}

private fun isSmsPermissionGranted(context: Context): Boolean =
    ContextCompat.checkSelfPermission(context, Manifest.permission.RECEIVE_SMS) ==
        PackageManager.PERMISSION_GRANTED

private fun isIgnoringBatteryOptimizations(context: Context): Boolean {
    val powerManager = context.getSystemService(PowerManager::class.java)
    return powerManager.isIgnoringBatteryOptimizations(context.packageName)
}
