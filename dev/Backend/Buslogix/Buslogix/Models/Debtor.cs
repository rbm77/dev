namespace Buslogix.Models
{
    public class Debtor
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? IdentityDocument { get; set; }
        public int GradeId { get; set; }
        public int RouteId { get; set; }
        public DateTime EntryDate { get; set; }
        public bool IsActive { get; set; }
        public decimal DueAmount { get; set; }
        public int PeriodsCount { get; set; }
        public int PaymentsCount { get; set; }
    }
}
