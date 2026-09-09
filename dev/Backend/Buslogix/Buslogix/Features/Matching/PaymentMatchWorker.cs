using Buslogix.Features.Matching.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Matching
{
    /// <summary>
    /// Consumes PaymentMatchRequest from IPaymentMatchQueue and tries to
    /// pair each one via IPaymentMatchingRepository.TryMatchAsync. Runs
    /// ConsumerCount loops concurrently - not because a single consumer
    /// can't keep up in the common case (matching is a fast local DB round
    /// trip, unlike MessageExtractionWorker's LLM fallback call), but as
    /// cheap insurance against bursts: an idle consumer costs nothing (no
    /// DI scope, no DB connection - both are only acquired once an item is
    /// actually dequeued), and correctness never depends on consumer count -
    /// that's guaranteed by the row-level FOR UPDATE locking inside
    /// match_payment_request_extraction, acquired in a fixed table order
    /// (payment_request then message_extraction_result) so concurrent calls
    /// never deadlock. When a burst does land (e.g. MessageExtractionWorker
    /// draining many extracted references at once), a second consumer lets
    /// MySQL process two different company/reference pairs in true
    /// parallel instead of queueing behind a single consumer.
    ///
    /// - Matched -> logged as Info (including whether it was also
    ///   auto-approved).
    /// - AmountMismatch -> logged as Warning: same reference exists on both
    ///   sides but with different amounts, a data anomaly worth surfacing
    ///   for manual reconciliation via the existing GET endpoints.
    /// - Neither -> no log; that's the normal outcome for most inserts and
    ///   would otherwise drown everything else.
    /// - Exception -> logged as Error and dropped. There is no retry
    ///   infrastructure elsewhere in this codebase, and a missed match is
    ///   not data loss: both underlying rows still exist until the next
    ///   relevant event, a manual validate, or the periodic match sweep
    ///   pairs them.
    /// </summary>
    public class PaymentMatchWorker(
        IPaymentMatchQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogHandler logHandler) : BackgroundService
    {
        private const int ConsumerCount = 2;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IEnumerable<Task> consumers = Enumerable.Range(0, ConsumerCount)
                .Select(_ => ConsumeAsync(stoppingToken));

            return Task.WhenAll(consumers);
        }

        private async Task ConsumeAsync(CancellationToken ct)
        {
            await foreach (PaymentMatchRequest request in queue.DequeueAllAsync(ct))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                try
                {
                    IPaymentMatchingRepository repository = scope.ServiceProvider.GetRequiredService<IPaymentMatchingRepository>();
                    PaymentMatchResult result = await repository.TryMatchAsync(request.CompanyId, request.Reference);

                    if (result.Matched)
                    {
                        await logHandler.WriteLog(
                            $"Matched payment_request {request.CompanyId}/{result.PaymentRequestId} against message_extraction_result reference '{request.Reference}'." +
                            (result.NewPaymentId > 0 ? $" Auto-approved as payment {result.NewPaymentId}." : ""),
                            LogType.Info);
                    }
                    else if (result.AmountMismatch)
                    {
                        await logHandler.WriteLog(
                            $"Payment match for company {request.CompanyId}, reference '{request.Reference}' found rows on both sides but the amounts differ - left unmatched for manual reconciliation.",
                            LogType.Warning);
                    }
                    // Neither matched nor mismatched: no counterpart yet, the normal case - nothing to log.
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    await logHandler.WriteLog(
                        $"Payment match worker error for company {request.CompanyId}, reference '{request.Reference}': {ex.Message}",
                        LogType.Error);
                }
            }
        }
    }
}
