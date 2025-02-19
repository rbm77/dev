namespace Buslogix.Models
{
    public class PaymentPeriodRequest
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public int DaysToNextPayment { get; set; }
        public bool IsPending { get; set; }
        public bool IsActive { get; set; }

        public PaymentPeriodRequest() { }
    }
}
