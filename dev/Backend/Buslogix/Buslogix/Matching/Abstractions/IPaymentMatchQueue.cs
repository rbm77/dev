using Buslogix.Matching;

namespace Buslogix.Matching.Abstractions
{
    /// <summary>
    /// In-memory hand-off point between the two insert paths that can
    /// produce a matchable pair (MessageExtractionWorker after saving a
    /// message_extraction_result row, PaymentRequestService after inserting
    /// a payment_request row) and PaymentMatchWorker, which consumes it in
    /// the background. Enqueueing never waits for the match attempt to run -
    /// it only hands the reference off.
    /// </summary>
    public interface IPaymentMatchQueue
    {
        ValueTask EnqueueAsync(PaymentMatchRequest request, CancellationToken ct = default);

        IAsyncEnumerable<PaymentMatchRequest> DequeueAllAsync(CancellationToken ct);
    }
}
