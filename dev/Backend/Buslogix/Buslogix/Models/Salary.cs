namespace Buslogix.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime? StartDate { get; set; }

        public Salary() { }
    }
}
