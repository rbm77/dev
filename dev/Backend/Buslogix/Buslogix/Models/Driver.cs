namespace Buslogix.Models
{
    public class Driver : Employee 
    {
        public string? LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }

        public Driver() { }
    }
}
