package com.bmsoftware.smsbridge.data.local

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query

@Dao
interface PendingSmsDao {

    @Insert
    suspend fun insert(sms: PendingSms): Long

    @Query("SELECT * FROM pending_sms WHERE id = :id")
    suspend fun getById(id: Long): PendingSms?

    @Query("SELECT * FROM pending_sms")
    suspend fun getAll(): List<PendingSms>

    @Query("UPDATE pending_sms SET attempts = 1 WHERE id = :id")
    suspend fun markRetry(id: Long)

    @Query("DELETE FROM pending_sms WHERE id = :id")
    suspend fun deleteById(id: Long)

    @Query("DELETE FROM pending_sms")
    suspend fun deleteAll()

    @Query("DELETE FROM pending_sms WHERE createdAt < :before")
    suspend fun deleteOlderThan(before: Long)
}
