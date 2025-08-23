namespace Buslogix.Models
{
    public class Employee : PersonalData
    {
        public int Id { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }

        public Employee() { }
    }
}
