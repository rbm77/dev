using static Buslogix.Utilities.Enums;

namespace Buslogix.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public IncidentType Type { get; set; }
        public string? CorrectiveActions { get; set; }

        public Incident() { }
    }
}
