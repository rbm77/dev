using Buslogix.Common.Llm;
using Buslogix.Features.WhatsAppBot.Abstractions;

namespace Buslogix.Features.WhatsAppBot.Llm
{
    public class WhatsAppReceiptImageExtractor(IChatCompletionClient chatCompletionClient) : IWhatsAppReceiptImageExtractor
    {
        private const string Prompt =
            "Extract the data from this payment receipt (a bank transfer or deposit to a school account): " +
            "the amount paid (amount), the reference or transaction/receipt number (reference), and the payment " +
            "date if it appears in the image (date, format yyyy-MM-dd, or null if no date appears). " +
            "The reference must be exactly one of these two values: the reference copied exactly as it appears, made only of " +
            "letters (A-Z, a-z) and digits (0-9), 1 to 50 characters, with no spaces, hyphens or other symbols; or null if the " +
            "receipt shows no reference, it is not fully legible, or it contains any character other than letters and digits. " +
            "Never return an empty string or placeholders such as \"N/A\", \"none\", \"unknown\" or \"not visible\".";

        public async Task<ExtractedData?> Extract(byte[] imageBytes, string mimeType, CancellationToken ct)
        {
            string? json = await chatCompletionClient.CompleteWithImage(imageBytes, mimeType, Prompt, ct);
            return JsonHelpers.TryDeserialize(json, out ExtractedData? data) ? data : null;
        }
    }
}
