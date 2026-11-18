namespace Buslogix.Features.PaymentRequests
{
    public class PaymentRequest
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal Amount { get; set; }
        public int StudentId { get; set; }
        public string? ReceiptReference { get; set; }
        public DateTime? RequestedAt { get; set; }
        public bool IsValidated { get; set; }

        public PaymentRequest() { }
    }
}
