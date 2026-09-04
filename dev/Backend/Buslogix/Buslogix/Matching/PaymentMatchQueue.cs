using System.Threading.Channels;
using Buslogix.Matching.Abstractions;

namespace Buslogix.Matching
{
    /// <summary>
    /// Unbounded in-memory queue backing IPaymentMatchQueue. Unbounded (and
    /// never dropping, unlike TriggerQueue) because a dropped match request
    /// would silently strand a matchable payment_request/message_extraction_result
    /// pair - each item is just a company id + reference, so holding many of
    /// them briefly is cheap. Registered as a singleton: one shared instance
    /// for the whole app.
    /// </summary>
    public class PaymentMatchQueue : IPaymentMatchQueue
    {
        private readonly Channel<PaymentMatchRequest> channel = Channel.CreateUnbounded<PaymentMatchRequest>();

        public ValueTask EnqueueAsync(PaymentMatchRequest request, CancellationToken ct = default) =>
            channel.Writer.WriteAsync(request, ct);

        public IAsyncEnumerable<PaymentMatchRequest> DequeueAllAsync(CancellationToken ct) =>
            channel.Reader.ReadAllAsync(ct);
    }
}
