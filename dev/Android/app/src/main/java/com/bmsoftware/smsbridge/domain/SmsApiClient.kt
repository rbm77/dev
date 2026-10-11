package com.bmsoftware.smsbridge.domain

interface SmsApiClient {
    suspend fun send(settings: AppSettings, text: String): SendResult
}
