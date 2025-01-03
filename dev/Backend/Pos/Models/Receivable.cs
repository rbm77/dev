using static Pos.Utilities.Enums;

namespace Pos.Models
{
    public class Receivable
    {
        public string? TransactionId { get; set; }

        public int BranchId { get; set; }

        public string? CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public Currency Currency { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
