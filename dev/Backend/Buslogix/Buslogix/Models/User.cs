namespace Buslogix.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public string? IdentityDocument { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public Role? Role { get; set; }
        public bool IsActive { get; set; }

        public User() { }
    }
}
