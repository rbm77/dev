using System.Threading.Channels;
using Buslogix.Features.PaymentMatching.Abstractions;

namespace Buslogix.Features.PaymentMatching
{
    public class PaymentMatchQueue : IPaymentMatchQueue
    {
        private readonly Channel<PaymentMatchRequest> channel = Channel.CreateUnbounded<PaymentMatchRequest>();

        public ValueTask Enqueue(PaymentMatchRequest request, CancellationToken ct = default) =>
            channel.Writer.WriteAsync(request, ct);

        public IAsyncEnumerable<PaymentMatchRequest> DequeueAll(CancellationToken ct) =>
            channel.Reader.ReadAllAsync(ct);
    }
}
