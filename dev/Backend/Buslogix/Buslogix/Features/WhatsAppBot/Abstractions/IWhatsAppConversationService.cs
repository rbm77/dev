using Buslogix.Features.WhatsAppBot.Conversation;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppConversationService
    {
        Task HandleInboundMessage(WhatsAppInboundMessage message, CancellationToken ct);
    }
}
