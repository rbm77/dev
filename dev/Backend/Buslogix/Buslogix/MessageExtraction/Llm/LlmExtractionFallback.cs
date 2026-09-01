using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Configuration;
using Microsoft.Extensions.Options;

namespace Buslogix.MessageExtraction.Llm
{
    /// <summary>
    /// Thin adapter: builds the prompt from the configured template, calls
    /// IChatCompletionClient, then parses the result with JsonHelpers. Contains no
    /// HttpClient calls or JSON-parsing logic of its own.
    /// </summary>
    public class LlmExtractionFallback(IChatCompletionClient chatCompletionClient, IOptionsMonitor<LlmFallbackOptions> optionsMonitor) : ILlmExtractionFallback
    {
        public async Task<ExtractedData?> ExtractAsync(string message, CancellationToken ct)
        {
            string prompt = optionsMonitor.CurrentValue.PromptTemplate.Replace("{{message}}", message);

            string? response = await chatCompletionClient.CompleteAsync(prompt, ct);

            return JsonHelpers.TryDeserialize(response, out ExtractedData? data) ? data : null;
        }
    }
}
