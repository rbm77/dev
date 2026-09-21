using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public class WhatsAppConversationWorker(
        IWhatsAppConversationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogHandler logHandler) : QueueConsumerWorker<WhatsAppInboundMessage>(queue, scopeFactory, logHandler)
    {
        protected override Task ProcessItem(IServiceProvider sp, WhatsAppInboundMessage message, CancellationToken ct)
        {
            IWhatsAppConversationService conversationService = sp.GetRequiredService<IWhatsAppConversationService>();
            return conversationService.HandleInboundMessage(message, ct);
        }

        protected override string DescribeError(WhatsAppInboundMessage message, Exception ex) =>
            $"WhatsApp conversation worker error processing a message from {message.WaId}: {ex.Message}";
    }
}
