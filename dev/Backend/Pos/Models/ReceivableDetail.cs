namespace Pos.Models
{
    public class ReceivableDetail
    {
        public int DetailId { get; set; }

        public string? TransactionId { get; set; }

        public string? ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
