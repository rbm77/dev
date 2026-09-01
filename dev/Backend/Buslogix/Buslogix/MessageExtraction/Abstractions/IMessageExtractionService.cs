namespace Buslogix.MessageExtraction.Abstractions
{
    public interface IMessageExtractionService
    {
        Task<ExtractedData?> ExtractAsync(string message, CancellationToken ct = default);
    }
}
