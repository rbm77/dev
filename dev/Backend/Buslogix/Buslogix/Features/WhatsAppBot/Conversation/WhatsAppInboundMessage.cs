using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public record WhatsAppInboundMessage(
        string PhoneNumberId,
        string WaId,
        WhatsAppMessageType Type,
        string? TextBody,
        string? MediaId,
        string? InteractiveReplyId,
        DateTime ReceivedAt);
}
