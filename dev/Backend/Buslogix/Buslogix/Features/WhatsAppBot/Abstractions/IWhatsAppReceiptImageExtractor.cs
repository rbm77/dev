using Buslogix.Common.Llm;

namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppReceiptImageExtractor
    {
        Task<ExtractedData?> Extract(byte[] imageBytes, string mimeType, CancellationToken ct);
    }
}
