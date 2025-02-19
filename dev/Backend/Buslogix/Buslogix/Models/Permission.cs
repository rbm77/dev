namespace Buslogix.Models
{
    public class Permission
    {
        public int RoleId { get; set; }
        public int ViewId { get; set; }
        public bool IsEditable { get; set; }

        public Permission() { }
    }
}
