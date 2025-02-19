namespace Buslogix.Models
{
    public class Restriction
    {
        public int UserId { get; set; }
        public int ViewId { get; set; }
        public bool IsFullyRestricted { get; set; }

        public Restriction() { }
    }
}
