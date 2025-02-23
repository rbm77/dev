namespace Buslogix.Models
{
    public static class Enums
    {
        public enum MaintenanceType
        {
            Preventive = 1,
            Corrective = 2,
            TechnicalInspection = 3
        }

        public enum IncidentType
        {
            Accident = 1,
            Breakdown = 2,
            Delay = 3,
            TrafficViolation = 4,
            SecurityEvent = 5
        }
    }
}
