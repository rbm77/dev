using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using static Buslogix.Utilities.Enums;

namespace Buslogix.MessageExtraction
{
    /// <summary>
    /// Orchestrates the two-tier extraction pipeline: tries every registered Tier 1
    /// parser first (fast, deterministic, no network), and only falls back to the
    /// Tier 2 LLM when nothing matched. Does not implement any automatic pattern
    /// learning/promotion - a Tier 2 hit is logged so a human can decide whether to
    /// add a new pattern to the configuration file.
    /// </summary>
    public class MessageExtractor(
        IEnumerable<IMessageParser> parsers,
        ILlmExtractionFallback llmExtractionFallback,
        ILogHandler logHandler) : IMessageExtractionService
    {
        public async Task<ExtractedData?> ExtractAsync(string message, CancellationToken ct = default)
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

            ExtractedData? fallbackResult = await llmExtractionFallback.ExtractAsync(message, ct);
            await logHandler.WriteLog($"Tier 2 LLM fallback result: {fallbackResult}", LogType.Info);

            return fallbackResult;
        }
    }
}
