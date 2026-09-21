using Buslogix.Common.BackgroundJobs;
using Buslogix.Features.PaymentMatching.Abstractions;

namespace Buslogix.Features.PaymentMatching
{
    public class PaymentMatchQueue : ItemQueue<PaymentMatchRequest>, IPaymentMatchQueue;
}
