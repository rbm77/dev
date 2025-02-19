namespace Buslogix.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int StudentId { get; set; }

        public Payment() { }
    }
}
