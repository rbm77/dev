using Buslogix.Features.PaymentMatching.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.PaymentMatching
{
    public class PaymentMatchWorker(
        IPaymentMatchQueue queue,
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
            await foreach (PaymentMatchRequest request in queue.DequeueAll(ct))
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                try
                {
                    IPaymentMatchingRepository repository = scope.ServiceProvider.GetRequiredService<IPaymentMatchingRepository>();
                    PaymentMatchResult result = await repository.TryMatch(request.CompanyId, request.Reference);

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
