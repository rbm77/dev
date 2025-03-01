namespace Buslogix.Models
{
    public class FuelExpense
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public decimal Liters { get; set; }
        public decimal Amount { get; set; }

        public FuelExpense() { }
    }
}
