namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>Row counts deleted by purge_message_extraction_history.</summary>
    public record MessageExtractionPurgeResult(int FailuresDeletedCount, int ResultsDeletedCount);
}
