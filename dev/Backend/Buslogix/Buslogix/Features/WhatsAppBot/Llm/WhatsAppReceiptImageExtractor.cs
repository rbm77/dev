using Buslogix.Common.Llm;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.Llm
{
    public class WhatsAppReceiptImageExtractor(IChatCompletionClient chatCompletionClient) : IWhatsAppReceiptImageExtractor
    {
        private const string Prompt =
            "Extract the data from this payment receipt (a bank transfer or deposit to a school account): " +
            "the amount paid (amount), the reference or transaction/receipt number (reference), and the payment " +
            "date if it appears in the image (date, format yyyy-MM-dd, or null if no date appears).";

        public async Task<ExtractedData?> Extract(byte[] imageBytes, string mimeType, CancellationToken ct)
        {
            string? json = await chatCompletionClient.CompleteWithImage(imageBytes, mimeType, Prompt, ct);
            return JsonHelpers.TryDeserialize(json, out ExtractedData? data) ? data : null;
        }
    }
}
