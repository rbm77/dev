namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface ILlmExtractionFallback
    {
        Task<ExtractedData?> ExtractAsync(string message, CancellationToken ct);
    }
}
