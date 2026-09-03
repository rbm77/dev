namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// Persists a successfully extracted result (Tier 1 pattern match or
    /// Tier 2 LLM fallback) - only called when ExtractAsync returns non-null.
    /// </summary>
    public interface IMessageExtractionResultRepository
    {
        Task InsertAsync(int companyId, ExtractedData data);
    }
}
