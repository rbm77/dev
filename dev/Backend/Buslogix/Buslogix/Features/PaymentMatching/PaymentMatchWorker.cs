using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.PaymentMatching.Abstractions;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.PaymentMatching
{
    public class PaymentMatchWorker(
        IPaymentMatchQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogHandler logHandler) : QueueConsumerWorker<PaymentMatchRequest>(queue, scopeFactory, logHandler, consumerCount: 2)
    {
        protected override async Task ProcessItem(IServiceProvider sp, PaymentMatchRequest request, CancellationToken ct)
        {
            IPaymentMatchingRepository repository = sp.GetRequiredService<IPaymentMatchingRepository>();

            PaymentMatchResult result = await repository.TryMatch(request.CompanyId, request.Reference);

            if (result.Matched)
            {
                await LogHandler.WriteLog(
                    $"Matched payment_request {request.CompanyId}/{result.PaymentRequestId} against message_extraction_result reference '{request.Reference}'." +
                    (result.NewPaymentId > 0 ? $" Auto-approved as payment {result.NewPaymentId}." : ""),
                    LogType.Info);
            }
            else if (result.AmountMismatch)
            {
                await LogHandler.WriteLog(
                    $"Payment match for company {request.CompanyId}, reference '{request.Reference}' found rows on both sides but the amounts differ - left unmatched for manual reconciliation.",
                    LogType.Warning);
            }
        }

        protected override string DescribeError(PaymentMatchRequest request, Exception ex) =>
            $"Payment match worker error for company {request.CompanyId}, reference '{request.Reference}': {ex.Message}";
    }
}
