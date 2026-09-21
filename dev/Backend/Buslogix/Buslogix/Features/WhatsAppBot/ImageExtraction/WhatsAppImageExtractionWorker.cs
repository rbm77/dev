using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.ImageExtraction
{
    public class WhatsAppImageExtractionWorker(
        IWhatsAppImageExtractionQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogHandler logHandler) : QueueConsumerWorker<WhatsAppImageExtractionItem>(queue, scopeFactory, logHandler, consumerCount: 2)
    {
        protected override Task ProcessItem(IServiceProvider sp, WhatsAppImageExtractionItem item, CancellationToken ct)
        {
            IWhatsAppReceiptProcessor receiptProcessor = sp.GetRequiredService<IWhatsAppReceiptProcessor>();
            return receiptProcessor.Process(item, ct);
        }

        protected override string DescribeError(WhatsAppImageExtractionItem item, Exception ex) =>
            $"WhatsApp image extraction worker error for company {item.CompanyId}, WaId {item.WaId}: {ex.Message}";
    }
}
