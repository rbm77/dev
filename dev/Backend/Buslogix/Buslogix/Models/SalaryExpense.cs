namespace Buslogix.Models
{
    public class SalaryExpense
    {
        public int Id { get; set; }
        public int SalaryId { get; set; }
        public DateTime Date { get; set; }

        public SalaryExpense() { }
    }
}
