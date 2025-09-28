namespace Buslogix.Models
{
    public class FuelExpense : Expense
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public int Liters { get; set; }

        public FuelExpense() { }
    }
}
