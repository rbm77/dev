using Buslogix.Common.BackgroundJobs;

namespace Buslogix.Features.Matching
{
    /// <summary>Trigger for PaymentRequestsController's "match-pending" endpoint.</summary>
    public class PaymentMatchSweepQueue : TriggerQueue;
}
