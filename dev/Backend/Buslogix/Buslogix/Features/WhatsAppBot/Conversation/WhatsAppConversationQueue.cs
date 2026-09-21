using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public class WhatsAppConversationQueue : ItemQueue<WhatsAppInboundMessage>, IWhatsAppConversationQueue;
}
