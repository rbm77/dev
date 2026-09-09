namespace Buslogix.Features.PaymentPeriodRequests
{
    public class PaymentPeriodRequest
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public int DaysToNextPayment { get; set; }

        public PaymentPeriodRequest() { }
    }
}
