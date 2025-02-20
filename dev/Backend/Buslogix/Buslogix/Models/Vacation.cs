namespace Buslogix.Models
{
    public class Vacation
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Vacation() { }
    }
}
