using Buslogix.Interfaces;
using Buslogix.Matching;
using Buslogix.Matching.Abstractions;
using Buslogix.MessageExtraction.Abstractions;
using static Buslogix.Utilities.Enums;

namespace Buslogix.MessageExtraction.Queue
{
    /// <summary>
    /// Consumes IngestionItem from IMessageIngestionQueue and runs them
    /// through IMessageExtractionService, independent of whether they came
    /// from email or SMS. Runs ConsumerCount loops concurrently - extraction
    /// is mostly spent waiting on the LLM fallback's network call, so a
    /// handful of concurrent consumers raises throughput without meaningfully
    /// competing for CPU. Each item gets its own DI scope, created fresh from
    /// IServiceScopeFactory, since it runs outside any HTTP request scope.
    ///
    /// - Extraction succeeds (non-null) -> recorded in message_extraction_result.
    /// - Extraction cleanly finds nothing (null) -> not an error, nothing to do
    ///   (MessageExtractor already logs this case internally).
    /// - Reference already exists (message_extraction_result or
    ///   payment_receipt_reference) -> not an error, just logged as a
    ///   duplicate and dropped; nothing is written to message_extraction_failure.
    /// - Extraction throws (LLM unreachable, unexpected error) -> logged and
    ///   the raw text is preserved in message_extraction_failure so it isn't
    ///   silently lost - unlike the email source, once a message is queued
    ///   here it will not be retried automatically.
    /// - Extraction throws for an item whose Source is Retry (requeued by
    ///   the message-extraction retry trigger) -> logged and dropped, NOT
    ///   re-inserted into message_extraction_failure. A retry already got
    ///   its second chance; re-inserting it would create an infinite
    ///   retry -> fail -> retry loop across trigger runs.
    /// </summary>
    public class MessageExtractionWorker(
        IMessageIngestionQueue queue,
        IPaymentMatchQueue paymentMatchQueue,
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
            await foreach (IngestionItem item in queue.DequeueAllAsync(ct))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                try
                {
                    IMessageExtractionService extractionService = scope.ServiceProvider.GetRequiredService<IMessageExtractionService>();
                    ExtractedData? extracted = await extractionService.ExtractAsync(item.RawText, ct);

                    if (extracted is not null)
                    {
                        IMessageExtractionResultRepository resultRepository = scope.ServiceProvider.GetRequiredService<IMessageExtractionResultRepository>();
                        await resultRepository.InsertAsync(item.CompanyId, extracted);

                        // Saved successfully (no DuplicateReferenceException) - try to
                        // pair it against a pending payment_request in the background,
                        // without holding up this consumer loop.
                        await paymentMatchQueue.EnqueueAsync(new PaymentMatchRequest(item.CompanyId, extracted.Reference), ct);
                    }
                    // extracted is null: no pattern/LLM match, not an error - nothing to persist.
                }
                catch (DuplicateReferenceException ex)
                {
                    await logHandler.WriteLog(
                        $"Message extraction worker skipped a {item.Source} message for company {item.CompanyId}: duplicate reference '{ex.Reference}'.",
                        LogType.Warning);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    await logHandler.WriteLog(
                        $"Message extraction worker error processing a {item.Source} message for company {item.CompanyId}: {ex.Message}",
                        LogType.Error);

                    // A retry that fails again does not go back into
                    // message_extraction_failure - it already got its second
                    // chance; re-inserting it would create an infinite
                    // retry -> fail -> retry loop. First-time failures
                    // (Email/Sms) are still preserved as before.
                    if (item.Source != IngestionSource.Retry)
                    {
                        IMessageExtractionFailureRepository failureRepository = scope.ServiceProvider.GetRequiredService<IMessageExtractionFailureRepository>();
                        await failureRepository.InsertAsync(item.CompanyId, item.RawText);
                    }
                }
            }
        }
    }
}
