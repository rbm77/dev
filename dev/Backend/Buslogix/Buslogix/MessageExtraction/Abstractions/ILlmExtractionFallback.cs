namespace Buslogix.MessageExtraction.Abstractions
{
    public interface ILlmExtractionFallback
    {
        Task<ExtractedData?> ExtractAsync(string message, CancellationToken ct);
    }
}
