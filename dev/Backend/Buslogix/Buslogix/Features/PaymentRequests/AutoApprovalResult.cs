namespace Buslogix.Features.PaymentRequests
{
    public class AutoApprovalResult
    {
        public int ProcessedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int FailedCount { get; set; }

        public AutoApprovalResult() { }
    }
}
