namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IChatCompletionClient
    {
        Task<string?> Complete(string prompt, CancellationToken ct);
    }
}
