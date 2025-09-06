namespace Buslogix.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string? LicensePlate { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int ManufactureYear { get; set; }
        public int Capacity { get; set; }
        public int Mileage { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public bool IsActive { get; set; }

        public Vehicle() { }
    }
}
