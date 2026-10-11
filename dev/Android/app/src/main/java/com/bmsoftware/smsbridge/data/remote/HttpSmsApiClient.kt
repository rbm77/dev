package com.bmsoftware.smsbridge.data.remote

import android.content.Context
import android.net.ConnectivityManager
import android.net.NetworkCapabilities
import com.bmsoftware.smsbridge.domain.AppSettings
import com.bmsoftware.smsbridge.domain.SendResult
import com.bmsoftware.smsbridge.domain.SmsApiClient
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONObject
import java.io.IOException
import java.net.HttpURLConnection
import java.net.URL

class HttpSmsApiClient(context: Context) : SmsApiClient {

    private val connectivityManager: ConnectivityManager? =
        context.applicationContext.getSystemService(ConnectivityManager::class.java)

    override suspend fun send(settings: AppSettings, text: String): SendResult =
        withContext(Dispatchers.IO) {
            val connection = try {
                val url = URL(settings.url)
                // Solo se acepta HTTPS; cualquier otra URL es un fallo normal.
                if (url.protocol != "https") return@withContext SendResult.Rejected
                url.openConnection() as HttpURLConnection
            } catch (e: Exception) {
                return@withContext SendResult.Rejected
            }

            try {
                connection.requestMethod = "POST"
                connection.connectTimeout = CONNECT_TIMEOUT_MS
                connection.readTimeout = READ_TIMEOUT_MS
                connection.doOutput = true
                // Una redirección no debe cambiar el método (POST → GET) en silencio.
                // Cualquier 3xx cuenta como rechazo.
                connection.instanceFollowRedirects = false
                connection.setRequestProperty("Content-Type", "application/json")
                connection.setRequestProperty("X-Api-Key", settings.apiKey)

                val body = JSONObject()
                    .put("companyId", settings.companyId)
                    .put("messageText", text)
                    .toString()
                connection.outputStream.use { it.write(body.toByteArray(Charsets.UTF_8)) }

                // Solo se lee el código; el cuerpo de la respuesta no se consume.
                if (connection.responseCode == HttpURLConnection.HTTP_ACCEPTED) {
                    SendResult.Accepted
                } else {
                    SendResult.Rejected
                }
            } catch (e: IOException) {
                if (hasInternet()) SendResult.Rejected else SendResult.NoConnection
            } finally {
                connection.disconnect()
            }
        }

    private fun hasInternet(): Boolean {
        val cm = connectivityManager ?: return false
        val capabilities = cm.getNetworkCapabilities(cm.activeNetwork) ?: return false
        return capabilities.hasCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET)
    }

    private companion object {
        const val CONNECT_TIMEOUT_MS = 10_000
        const val READ_TIMEOUT_MS = 15_000
    }
}
