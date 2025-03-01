namespace Buslogix.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }

        public Salary() { }
    }
}
