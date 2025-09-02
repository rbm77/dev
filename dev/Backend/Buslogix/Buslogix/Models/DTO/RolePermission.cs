namespace Buslogix.Models.DTO
{
    public class RolePermission
    {

        public int ResourceId { get; set; }
        public string? Description { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsEditable { get; set; }
        
        public RolePermission() { }
    }
}
