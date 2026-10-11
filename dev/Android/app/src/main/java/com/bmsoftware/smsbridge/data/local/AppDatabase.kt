package com.bmsoftware.smsbridge.data.local

import androidx.room.Database
import androidx.room.RoomDatabase

@Database(entities = [PendingSms::class], version = 1, exportSchema = false)
abstract class AppDatabase : RoomDatabase() {
    abstract fun pendingSmsDao(): PendingSmsDao
}
