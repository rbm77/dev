using Buslogix.Common.Llm;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageExtractionService
    {
        Task<ExtractedData?> Extract(string message, CancellationToken ct = default);
    }
}
