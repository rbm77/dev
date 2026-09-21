using Buslogix.Features.WhatsAppBot.ImageExtraction;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppReceiptProcessor
    {
        Task Process(WhatsAppImageExtractionItem item, CancellationToken ct);
    }
}
