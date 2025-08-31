namespace Buslogix.Models.DTO
{
    public class NotificationData
    {
        public int CompanyId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public NotificationData() { }
    }
}
