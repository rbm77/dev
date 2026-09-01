namespace Buslogix.Models.DTO
{
    public class SchedulePaymentPeriodsResult
    {
        public int ProcessedCount { get; set; }
        public int ScheduledCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }

        public SchedulePaymentPeriodsResult() { }
    }
}
