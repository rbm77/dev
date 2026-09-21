namespace Buslogix.Common.Llm
{
    public interface IChatCompletionClient
    {
        Task<string?> Complete(string prompt, CancellationToken ct);

        Task<string?> CompleteWithImage(byte[] imageBytes, string mediaType, string prompt, CancellationToken ct);
    }
}
