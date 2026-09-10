using Buslogix.Features.PaymentMatching;

namespace Buslogix.Features.PaymentMatching.Abstractions
{
    public interface IPaymentMatchQueue
    {
        ValueTask Enqueue(PaymentMatchRequest request, CancellationToken ct = default);

        IAsyncEnumerable<PaymentMatchRequest> DequeueAll(CancellationToken ct);
    }
}
