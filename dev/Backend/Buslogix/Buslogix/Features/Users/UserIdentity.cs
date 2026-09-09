namespace Buslogix.Features.Users
{
    public class UserIdentity
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string? Username { get; set; }
        public List<string>? Permissions { get; set; }
        public bool IsAuthenticated { get; set; }

        public UserIdentity() { }
    }
}
