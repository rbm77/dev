namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface ILlmExtractionFallback
    {
        Task<ExtractedData?> Extract(string message, CancellationToken ct);
    }
}
