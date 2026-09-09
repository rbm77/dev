namespace Buslogix.Features.Users
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
