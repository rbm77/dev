namespace Buslogix.Models
{
    public class Permission
    {
        public View? View { get; set; }
        public bool IsEditable { get; set; }

        public Permission() { }
    }
}
