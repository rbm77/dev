using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.Conversation;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppConversationQueue : IItemQueue<WhatsAppInboundMessage>;
}
