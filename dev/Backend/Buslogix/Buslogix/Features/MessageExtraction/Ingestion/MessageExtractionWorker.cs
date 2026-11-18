using Buslogix.Common.BackgroundJobs;
using Buslogix.Common.Llm;
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
        ILogHandler logHandler) : QueueConsumerWorker<IngestionItem>(queue, scopeFactory, logHandler, consumerCount: 2)
    {
        protected override async Task ProcessItem(IServiceProvider sp, IngestionItem item, CancellationToken ct)
        {
            try
            {
                IMessageExtractionService extractionService = sp.GetRequiredService<IMessageExtractionService>();
                ExtractedData? extracted = await extractionService.Extract(item.RawText, ct);

                if (extracted is not null)
                {
                    string? reference = ExtractedData.NormalizeReference(extracted.Reference);

                    if (reference is null)
                    {
                        await LogHandler.WriteLog(
                            $"Message extraction worker discarded a {item.Source} message for company {item.CompanyId}: no valid reference was extracted ('{extracted.Reference}').",
                            LogType.Warning);
                        return;
                    }

                    IMessageExtractionResultRepository resultRepository = sp.GetRequiredService<IMessageExtractionResultRepository>();
                    await resultRepository.Insert(item.CompanyId, extracted with { Reference = reference });

                    await paymentMatchQueue.Enqueue(new PaymentMatchRequest(item.CompanyId, reference), ct);
                }
            }
            catch (DuplicateReferenceException ex)
            {
                await LogHandler.WriteLog(
                    $"Message extraction worker skipped a {item.Source} message for company {item.CompanyId}: duplicate reference '{ex.Reference}'.",
                    LogType.Warning);
            }
            catch (Exception ex) when (ex is not OperationCanceledException && item.Source != IngestionSource.Retry)
            {
                IMessageExtractionFailureRepository failureRepository = sp.GetRequiredService<IMessageExtractionFailureRepository>();
                await failureRepository.Insert(item.CompanyId, item.RawText);
                throw;
            }
        }

        protected override string DescribeError(IngestionItem item, Exception ex) =>
            $"Message extraction worker error processing a {item.Source} message for company {item.CompanyId}: {ex.Message}";
    }
}
