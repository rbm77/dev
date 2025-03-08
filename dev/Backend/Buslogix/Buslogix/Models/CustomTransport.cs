namespace Buslogix.Models
{
    public class CustomTransport
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public CustomTransport() { }
    }
}
