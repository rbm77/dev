using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Configuration;
using Microsoft.Extensions.Options;

namespace Buslogix.Features.MessageExtraction.Llm
{
    public class LlmExtractionFallback(IChatCompletionClient chatCompletionClient, IOptionsMonitor<LlmFallbackOptions> optionsMonitor) : ILlmExtractionFallback
    {
        public async Task<ExtractedData?> Extract(string message, CancellationToken ct)
        {
            string prompt = optionsMonitor.CurrentValue.PromptTemplate.Replace("{{message}}", message);

            string? response = await chatCompletionClient.Complete(prompt, ct);

            return JsonHelpers.TryDeserialize(response, out ExtractedData? data) ? data : null;
        }
    }
}
