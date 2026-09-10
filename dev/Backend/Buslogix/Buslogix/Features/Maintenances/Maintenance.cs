using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.Maintenances
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? Description { get; set; }
        public MaintenanceType Type { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public Maintenance() { }
    }
}
