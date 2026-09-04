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

        /// <summary>
        /// Atomically reads and deletes every row currently in
        /// message_extraction_failure, for the retry trigger to requeue.
        /// The rows are gone from the table as soon as this returns -
        /// there is no separate delete step.
        /// </summary>
        Task<List<ClaimedExtractionFailure>> ClaimAllForRetryAsync();
    }
}
