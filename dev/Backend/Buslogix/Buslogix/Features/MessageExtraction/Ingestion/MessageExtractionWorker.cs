using Buslogix.Features.PaymentMatching;
using Buslogix.Features.PaymentMatching.Abstractions;
using Buslogix.Features.MessageExtraction.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction.Ingestion
{
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
                .Select(_ => Consume(stoppingToken));

            return Task.WhenAll(consumers);
        }

        private async Task Consume(CancellationToken ct)
        {
            await foreach (IngestionItem item in queue.DequeueAll(ct))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                try
                {
                    IMessageExtractionService extractionService = scope.ServiceProvider.GetRequiredService<IMessageExtractionService>();
                    ExtractedData? extracted = await extractionService.Extract(item.RawText, ct);

                    if (extracted is not null)
                    {
                        IMessageExtractionResultRepository resultRepository = scope.ServiceProvider.GetRequiredService<IMessageExtractionResultRepository>();
                        await resultRepository.Insert(item.CompanyId, extracted);

                        await paymentMatchQueue.Enqueue(new PaymentMatchRequest(item.CompanyId, extracted.Reference), ct);
                    }
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

                    if (item.Source != IngestionSource.Retry)
                    {
                        IMessageExtractionFailureRepository failureRepository = scope.ServiceProvider.GetRequiredService<IMessageExtractionFailureRepository>();
                        await failureRepository.Insert(item.CompanyId, item.RawText);
                    }
                }
            }
        }
    }
}
