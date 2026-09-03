namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// Preserves the raw text of a message whose extraction attempt errored
    /// out (LLM unreachable, unexpected exception) - not for a clean "no
    /// pattern matched" result, which is not an error and is not stored.
    /// </summary>
    public interface IMessageExtractionFailureRepository
    {
        Task InsertAsync(int companyId, string rawText);
    }
}
