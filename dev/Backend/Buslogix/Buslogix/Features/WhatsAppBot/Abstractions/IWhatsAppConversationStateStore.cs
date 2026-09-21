using Buslogix.Features.WhatsAppBot.Conversation;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppConversationStateStore
    {
        WhatsAppConversationState? Get(string waId);

        void Set(string waId, WhatsAppConversationState state);

        void Remove(string waId);
    }
}
