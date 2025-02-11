namespace Buslogix.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public Company() { }
    }
}
