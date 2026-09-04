namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// The two housekeeping operations behind the message-extraction trigger
    /// endpoints: retrying failed extractions and purging old history.
    /// </summary>
    public interface IMessageExtractionMaintenanceService
    {
        /// <summary>Requeues every currently-claimed failure and returns how many were requeued.</summary>
        Task<int> RetryFailedExtractionsAsync(CancellationToken ct);

        Task<MessageExtractionPurgeResult> PurgeExpiredRecordsAsync();
    }
}
