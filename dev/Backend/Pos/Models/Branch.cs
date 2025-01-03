namespace Pos.Models
{
    public class Branch
    {
        public int BranchId { get; set; }

        public int CompanyId { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
