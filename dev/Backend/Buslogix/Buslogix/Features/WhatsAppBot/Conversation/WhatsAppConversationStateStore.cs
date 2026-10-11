using Microsoft.Extensions.Caching.Memory;
using Buslogix.Features.WhatsAppBot.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public class WhatsAppConversationStateStore(IServiceScopeFactory scopeFactory) : IWhatsAppConversationStateStore, IDisposable
    {
        private const string InactivityMessage = "Tu solicitud fue cancelada por inactividad. Escribinos de nuevo para comenzar.";

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

        private readonly MemoryCache cache = new(new MemoryCacheOptions());

        public WhatsAppConversationState? Get(string waId) =>
            cache.TryGetValue(waId, out WhatsAppConversationState? state) ? state : null;

        public void Set(string waId, WhatsAppConversationState state)
        {
            MemoryCacheEntryOptions options = new()
            {
                SlidingExpiration = Ttl,
                Priority = CacheItemPriority.NeverRemove
            };
            options.RegisterPostEvictionCallback(OnEvicted);

            cache.Set(waId, state, options);
        }

        public void Remove(string waId) => cache.Remove(waId);

        public void Dispose() => cache.Dispose();

        private void OnEvicted(object key, object? value, EvictionReason reason, object? evictionState)
        {
            if (reason != EvictionReason.Expired || value is not WhatsAppConversationState state)
            {
                return;
            }

            _ = NotifyExpired(state);
        }

        private async Task NotifyExpired(WhatsAppConversationState state)
        {
            using IServiceScope scope = scopeFactory.CreateScope();

            try
            {
                IWhatsAppGraphClient graphClient = scope.ServiceProvider.GetRequiredService<IWhatsAppGraphClient>();
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, InactivityMessage, CancellationToken.None);
            }
            catch (Exception ex)
            {
                using IDisposable logScope = LogScope.Push(LogScope.NewId(), state.CompanyId);
                ILogHandler logHandler = scope.ServiceProvider.GetRequiredService<ILogHandler>();
                await logHandler.WriteLog($"WhatsApp conversation expiration: failed to notify {state.WaId}: {ex.Message}", LogType.Error);
            }
        }
    }
}
