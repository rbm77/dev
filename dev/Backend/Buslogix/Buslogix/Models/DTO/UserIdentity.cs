namespace Buslogix.Models.DTO
{
    public class UserIdentity
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public List<string>? Permissions { get; set; }
        public bool IsAuthenticated { get; set; }

        public UserIdentity() { }
    }
}
