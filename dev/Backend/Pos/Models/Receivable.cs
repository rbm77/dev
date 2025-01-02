using static Pos.Utilities.Enums;

namespace Pos.Models
{
    public class Receivable
    {
        public string? ReceivableId { get; set; }

        public string? TenantId { get; set; }

        public string? CustomerId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
