namespace Pos.Models
{
    public class Tax
    {
        public string? TaxId { get; set; }

        public int CompanyId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Rate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
