package com.bmsoftware.smsbridge.ui

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.bmsoftware.smsbridge.data.PendingSmsRepository
import com.bmsoftware.smsbridge.work.SmsWorkScheduler
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class MessageItem(
    val id: Long,
    val text: String,
    val attempts: Int,
    val createdAt: Long,
    val hasActiveWork: Boolean,
)

data class MessagesState(val items: List<MessageItem> = emptyList())

class MessagesViewModel(
    private val repository: PendingSmsRepository,
    private val scheduler: SmsWorkScheduler,
) : ViewModel() {

    private val _state = MutableStateFlow(MessagesState())
    val state: StateFlow<MessagesState> = _state.asStateFlow()

    init {
        refresh()
    }

    /** Vuelve a leer la base de datos y el estado de WorkManager. */
    fun refresh() {
        viewModelScope.launch {
            _state.value = MessagesState(items = load())
        }
    }

    /** Cancela los envíos del mensaje y después lo borra de la base de datos. */
    fun delete(id: Long) {
        viewModelScope.launch {
            scheduler.cancelFor(id)
            repository.delete(id)
            _state.value = MessagesState(items = load())
        }
    }

    private suspend fun load(): List<MessageItem> = repository.getAll()
        .sortedByDescending { it.createdAt }
        .map { sms ->
            MessageItem(
                id = sms.id,
                text = sms.text,
                attempts = sms.attempts,
                createdAt = sms.createdAt,
                hasActiveWork = scheduler.hasActiveWork(sms.id),
            )
        }
}
