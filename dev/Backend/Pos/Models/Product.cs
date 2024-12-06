namespace Pos.Models
{
    public class Product
    {
        public string? ProductId { get; set; }

        public string? TenantId { get; set; }

        public string? SKU { get; set; }

        public string? Barcode { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int MinimumStock { get; set; }

        public string? CategoryId { get; set; }

        public string? UnitId { get; set; }

        public string? SupplierId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
