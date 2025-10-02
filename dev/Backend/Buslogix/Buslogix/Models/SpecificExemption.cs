namespace Buslogix.Models
{
    public class SpecificExemption
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int PaymentPeriodId { get; set; }
        public string? Description { get; set; }
        public decimal Percentage { get; set; }

        public SpecificExemption() { }
    }
}
