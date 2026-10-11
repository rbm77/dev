package com.bmsoftware.smsbridge

import android.os.Bundle
import android.view.WindowManager
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.activity.viewModels
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.compose.LifecycleEventEffect
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.lifecycle.viewmodel.initializer
import androidx.lifecycle.viewmodel.viewModelFactory
import com.bmsoftware.smsbridge.ui.MessagesScreen
import com.bmsoftware.smsbridge.ui.MessagesViewModel
import com.bmsoftware.smsbridge.ui.PinScreen
import com.bmsoftware.smsbridge.ui.SessionViewModel
import com.bmsoftware.smsbridge.ui.SettingsScreen
import com.bmsoftware.smsbridge.ui.SettingsViewModel
import com.bmsoftware.smsbridge.ui.theme.SmsBridgeTheme

private enum class Screen { SETTINGS, MESSAGES }

class MainActivity : ComponentActivity() {

    private val session: SessionViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        // Impide capturas de pantalla y la miniatura de "recientes", que mostrarían la API Key.
        window.addFlags(WindowManager.LayoutParams.FLAG_SECURE)

        setContent {
            SmsBridgeTheme {
                val unlocked by session.unlocked.collectAsState()
                var screen by rememberSaveable { mutableStateOf(Screen.SETTINGS) }

                // Al salir de la app se vuelve a pedir el PIN y se regresa a Configuración.
                // Un giro de pantalla también dispara ON_STOP, por eso se ignora si la actividad se recrea.
                LifecycleEventEffect(Lifecycle.Event.ON_STOP) {
                    if (!isChangingConfigurations) {
                        session.lock()
                        screen = Screen.SETTINGS
                    }
                }

                if (!unlocked) {
                    PinScreen(onUnlock = session::unlock)
                } else {
                    val container = (application as SmsBridgeApp).container
                    when (screen) {
                        Screen.SETTINGS -> {
                            val settingsViewModel: SettingsViewModel = viewModel(
                                factory = viewModelFactory {
                                    initializer {
                                        SettingsViewModel(container.settingsRepository, container.activation)
                                    }
                                }
                            )
                            SettingsScreen(
                                viewModel = settingsViewModel,
                                onOpenMessages = { screen = Screen.MESSAGES },
                            )
                        }

                        Screen.MESSAGES -> {
                            BackHandler { screen = Screen.SETTINGS }
                            val messagesViewModel: MessagesViewModel = viewModel(
                                factory = viewModelFactory {
                                    initializer {
                                        MessagesViewModel(container.pendingSmsRepository, container.workScheduler)
                                    }
                                }
                            )
                            MessagesScreen(
                                viewModel = messagesViewModel,
                                onBack = { screen = Screen.SETTINGS },
                            )
                        }
                    }
                }
            }
        }
    }
}
