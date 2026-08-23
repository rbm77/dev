namespace Buslogix.Models
{
    public class PaymentRequest
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal Amount { get; set; }
        public int StudentId { get; set; }
        public string ReceiptReference { get; set; } = string.Empty;
        public DateTime? RequestedAt { get; set; }
        public bool IsValidated { get; set; }

        public PaymentRequest() { }
    }
}
