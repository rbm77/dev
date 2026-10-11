using Buslogix.Common.Llm;
using Buslogix.Features.MessageExtraction.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction
{
    public class MessageExtractor(
        IEnumerable<IMessageParser> parsers,
        ILlmExtractionFallback llmExtractionFallback,
        ILogHandler logHandler) : IMessageExtractionService
    {
        public async Task<ExtractedData?> Extract(string message, CancellationToken ct = default)
        {
            foreach (IMessageParser parser in parsers)
            {
                if (parser.TryParse(message, out ExtractedData? data))
                {
                    return data;
                }
            }

            ExtractedData? fallbackResult = await llmExtractionFallback.Extract(message, ct);

            if (fallbackResult is null)
            {
                await logHandler.WriteLog("Message extraction: Tier 2 LLM fallback failed to produce a result.", LogType.Warning);
            }

            return fallbackResult;
        }
    }
}
