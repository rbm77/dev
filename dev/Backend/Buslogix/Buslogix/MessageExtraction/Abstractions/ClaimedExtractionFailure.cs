namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// A message_extraction_failure row claimed for retry - by the time this
    /// is returned from ClaimAllForRetryAsync, the row is already gone from
    /// the table (see retry_message_extraction_failures).
    /// </summary>
    public record ClaimedExtractionFailure(int CompanyId, string RawText, DateTime ReceivedAt);
}
