using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Queue;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction
{
    public class MessageExtractionMaintenanceService(
        IMessageExtractionFailureRepository failureRepository,
        IMessageExtractionHistoryRepository historyRepository,
        IMessageIngestionQueue ingestionQueue) : IMessageExtractionMaintenanceService
    {
        public async Task<int> RetryFailedExtractionsAsync(CancellationToken ct)
        {
            List<ClaimedExtractionFailure> claimed = await failureRepository.ClaimAllForRetryAsync();

            foreach (ClaimedExtractionFailure failure in claimed)
            {
                await ingestionQueue.EnqueueAsync(
                    new IngestionItem(IngestionSource.Retry, failure.CompanyId, failure.RawText, failure.ReceivedAt), ct);
            }

            return claimed.Count;
        }

        public Task<MessageExtractionPurgeResult> PurgeExpiredRecordsAsync() => historyRepository.PurgeExpiredRecordsAsync();
    }
}
