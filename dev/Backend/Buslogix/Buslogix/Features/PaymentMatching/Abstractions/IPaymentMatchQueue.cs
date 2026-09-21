using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.PaymentMatching;

namespace Buslogix.Features.PaymentMatching.Abstractions
{
    public interface IPaymentMatchQueue : IItemQueue<PaymentMatchRequest>;
}
