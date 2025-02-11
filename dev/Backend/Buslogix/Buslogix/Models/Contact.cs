namespace Buslogix.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public Contact() { }
    }
}
