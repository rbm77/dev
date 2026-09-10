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
                    await logHandler.WriteLog($"Message extracted by Tier 1 parser {parser.GetType().Name}: {data}", LogType.Info);
                    return data;
                }
            }

            await logHandler.WriteLog($"No Tier 1 pattern matched message, falling back to LLM. RawMessage: {message}", LogType.Warning);

            ExtractedData? fallbackResult = await llmExtractionFallback.Extract(message, ct);
            await logHandler.WriteLog($"Tier 2 LLM fallback result: {fallbackResult}", LogType.Info);

            return fallbackResult;
        }
    }
}
