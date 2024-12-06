namespace Pos.Models
{
    public class Supplier
    {
        public string? SupplierId { get; set; }

        public string? TenantId { get; set; }

        public string? Name { get; set; }
  
        public string? ContactInfo { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
