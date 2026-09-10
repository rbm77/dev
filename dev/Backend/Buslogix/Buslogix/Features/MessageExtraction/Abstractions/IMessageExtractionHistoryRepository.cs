namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageExtractionHistoryRepository
    {
        Task<MessageExtractionPurgeResult> PurgeExpiredRecords();
    }
}
