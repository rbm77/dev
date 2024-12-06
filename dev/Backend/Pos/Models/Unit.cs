namespace Pos.Models
{
    public class Unit
    {
        public string? UnitId { get; set; }

        public string? TenantId { get; set; }

        public string? Name { get; set; }

        public string? Abbreviation { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
