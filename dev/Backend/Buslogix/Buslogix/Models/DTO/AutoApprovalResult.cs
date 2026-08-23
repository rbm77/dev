namespace Buslogix.Models.DTO
{
    public class AutoApprovalResult
    {
        public int ProcessedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int FailedCount { get; set; }

        public AutoApprovalResult() { }
    }
}
