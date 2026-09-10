using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Ingestion;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction
{
    public class MessageExtractionMaintenanceService(
        IMessageExtractionFailureRepository failureRepository,
        IMessageExtractionHistoryRepository historyRepository,
        IMessageIngestionQueue ingestionQueue) : IMessageExtractionMaintenanceService
    {
        public async Task<int> RetryFailedExtractions(CancellationToken ct)
        {
            List<ClaimedExtractionFailure> claimed = await failureRepository.ClaimAllForRetry();

            foreach (ClaimedExtractionFailure failure in claimed)
            {
                await ingestionQueue.Enqueue(
                    new IngestionItem(IngestionSource.Retry, failure.CompanyId, failure.RawText, failure.ReceivedAt), ct);
            }

            return claimed.Count;
        }

        public Task<MessageExtractionPurgeResult> PurgeExpiredRecords() => historyRepository.PurgeExpiredRecords();
    }
}
