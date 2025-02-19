namespace Buslogix.Models
{
    public class PaymentPeriod
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsProcessed { get; set; }

        public PaymentPeriod() { }
    }
}
