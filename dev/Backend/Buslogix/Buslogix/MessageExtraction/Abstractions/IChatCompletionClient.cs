namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// Sends a prompt to Claude and returns the raw text response. Kept behind its
    /// own interface for testability and to isolate this feature's one external LLM
    /// dependency (currently the Anthropic SDK) from the rest of the module.
    /// </summary>
    public interface IChatCompletionClient
    {
        Task<string?> CompleteAsync(string prompt, CancellationToken ct);
    }
}
