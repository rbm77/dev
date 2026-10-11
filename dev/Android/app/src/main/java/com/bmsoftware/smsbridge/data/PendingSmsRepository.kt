package com.bmsoftware.smsbridge.data

import com.bmsoftware.smsbridge.data.local.PendingSms
import com.bmsoftware.smsbridge.data.local.PendingSmsDao

/** Fachada sobre el DAO de SMS pendientes. */
class PendingSmsRepository(private val dao: PendingSmsDao) {

    suspend fun save(text: String): Long = dao.insert(PendingSms(text = text))

    suspend fun get(id: Long): PendingSms? = dao.getById(id)

    suspend fun getAll(): List<PendingSms> = dao.getAll()

    suspend fun markRetry(id: Long) = dao.markRetry(id)

    suspend fun delete(id: Long) = dao.deleteById(id)

    suspend fun deleteAll() = dao.deleteAll()

    suspend fun deleteOlderThan(timestampMs: Long) = dao.deleteOlderThan(timestampMs)
}
