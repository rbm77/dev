namespace Buslogix.Features.MessageExtraction.Abstractions
{
    public interface IMessageExtractionMaintenanceService
    {
        Task<int> RetryFailedExtractions(CancellationToken ct);

        Task<MessageExtractionPurgeResult> PurgeExpiredRecords();
    }
}
