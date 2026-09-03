using Buslogix.Interfaces;
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
    /// - Extraction throws (LLM unreachable, unexpected error) -> logged and
    ///   the raw text is preserved in message_extraction_failure so it isn't
    ///   silently lost - unlike the email source, once a message is queued
    ///   here it will not be retried automatically.
    /// </summary>
    public class MessageExtractionWorker(
        IMessageIngestionQueue queue,
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
                    }
                    // extracted is null: no pattern/LLM match, not an error - nothing to persist.
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    await logHandler.WriteLog(
                        $"Message extraction worker error processing a {item.Source} message for company {item.CompanyId}: {ex.Message}",
                        LogType.Error);

                    IMessageExtractionFailureRepository failureRepository = scope.ServiceProvider.GetRequiredService<IMessageExtractionFailureRepository>();
                    await failureRepository.InsertAsync(item.CompanyId, item.RawText);
                }
            }
        }
    }
}
