namespace Buslogix.Models
{
    public class Expense
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public Expense() { }
    }
}
