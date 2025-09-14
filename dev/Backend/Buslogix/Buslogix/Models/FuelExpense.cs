namespace Buslogix.Models
{
    public class FuelExpense
    {
        public long ExpenseId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public decimal Liters { get; set; }

        public FuelExpense() { }
    }
}
