namespace Buslogix.Models
{
    public class User : PersonalData
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }

        public User() { }
    }
}
