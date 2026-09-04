namespace Buslogix.MessageExtraction.Abstractions
{
    /// <summary>
    /// Housekeeping over message_extraction_failure and
    /// message_extraction_result together - spans both tables, so it does
    /// not belong to either single-table repository.
    /// </summary>
    public interface IMessageExtractionHistoryRepository
    {
        /// <summary>
        /// Deletes failure/result records older than the retention window
        /// (3 days, enforced in purge_message_extraction_history) and
        /// returns how many rows were removed from each table.
        /// </summary>
        Task<MessageExtractionPurgeResult> PurgeExpiredRecordsAsync();
    }
}
