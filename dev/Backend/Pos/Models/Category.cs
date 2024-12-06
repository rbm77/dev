namespace Pos.Models
{
    public class Category
    {
        public string? CategoryId { get; set; }

        public string? TenantId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
