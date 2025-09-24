namespace Buslogix.Models
{
    public class CustomTransport
    {
        public int Id { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public CustomTransport() { }
    }
}
