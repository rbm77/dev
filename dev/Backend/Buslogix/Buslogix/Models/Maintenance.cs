using static Buslogix.Utilities.Enums;

namespace Buslogix.Models
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public MaintenanceType Type { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public decimal Amount { get; set; }
 
        public Maintenance() { }
    }
}
