package com.bmsoftware.smsbridge.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey

/**
 * SMS pendiente de envío.
 * attempts: 0 = primer intento, 1 = reintento ya programado.
 * createdAt: epoch en ms, para descartar filas demasiado antiguas.
 */
@Entity(tableName = "pending_sms")
data class PendingSms(
    @PrimaryKey(autoGenerate = true) val id: Long = 0,
    val text: String,
    val attempts: Int = 0,
    val createdAt: Long = System.currentTimeMillis(),
)
