using Buslogix.Common.BackgroundJobs;

namespace Buslogix.Features.PaymentRequests
{
    /// <summary>Trigger for PaymentRequestsController's "auto-approve" endpoint.</summary>
    public class PaymentAutoApprovalQueue : TriggerQueue;
}
