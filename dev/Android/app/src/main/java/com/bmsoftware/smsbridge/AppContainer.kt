package com.bmsoftware.smsbridge

import android.content.Context
import androidx.room.Room
import androidx.work.WorkManager
import com.bmsoftware.smsbridge.data.PendingSmsRepository
import com.bmsoftware.smsbridge.data.SettingsRepository
import com.bmsoftware.smsbridge.data.local.AppDatabase
import com.bmsoftware.smsbridge.data.remote.HttpSmsApiClient
import com.bmsoftware.smsbridge.domain.SmsApiClient
import com.bmsoftware.smsbridge.work.SmsWorkScheduler
import kotlinx.coroutines.CoroutineScope

/** Composition root manual. Cada dependencia se crea la primera vez que se usa. */
class AppContainer(context: Context, private val appScope: CoroutineScope) {
    private val appContext = context.applicationContext

    val database: AppDatabase by lazy {
        Room.databaseBuilder(appContext, AppDatabase::class.java, "smsbridge.db").build()
    }

    val pendingSmsRepository: PendingSmsRepository by lazy {
        PendingSmsRepository(database.pendingSmsDao())
    }

    val settingsRepository: SettingsRepository by lazy { SettingsRepository(appContext) }

    val smsApiClient: SmsApiClient by lazy { HttpSmsApiClient(appContext) }

    val workScheduler: SmsWorkScheduler by lazy {
        SmsWorkScheduler(WorkManager.getInstance(appContext))
    }

    val activation: ActivationController by lazy {
        ActivationController(
            context = appContext,
            settingsRepository = settingsRepository,
            pendingSmsRepository = pendingSmsRepository,
            scheduler = workScheduler,
            appScope = appScope,
        )
    }
}
