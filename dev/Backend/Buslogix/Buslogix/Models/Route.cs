﻿namespace Buslogix.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public Route() { }
    }
}
