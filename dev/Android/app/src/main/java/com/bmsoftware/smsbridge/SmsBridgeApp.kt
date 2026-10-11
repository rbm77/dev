package com.bmsoftware.smsbridge

import android.app.Application
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob

class SmsBridgeApp : Application() {

    /** Scope único para el trabajo corto que disparan el receptor y el controlador. Vive con el proceso. */
    val appScope: CoroutineScope = CoroutineScope(SupervisorJob() + Dispatchers.IO)

    val container: AppContainer by lazy { AppContainer(this, appScope) }

    override fun onCreate() {
        super.onCreate()
        // Se ejecuta cada vez que nace el proceso, también cuando Android lo levanta para un SMS.
        container.activation.syncOnStart()
    }
}
