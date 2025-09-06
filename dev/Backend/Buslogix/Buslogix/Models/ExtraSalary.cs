namespace Buslogix.Models
{
    public class ExtraSalary
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public ExtraSalary() { }
    }
}
