namespace Buslogix.Models
{
    public class SalaryAdjustment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public SalaryAdjustment() { }
    }
}
