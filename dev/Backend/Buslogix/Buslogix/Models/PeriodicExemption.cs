﻿namespace Buslogix.Models
{
    public class PeriodicExemption
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Percentage { get; set; }

        public PeriodicExemption() { }
    }
}
