namespace Buslogix.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? IdentityDocument { get; set; }
        public int GradeId { get; set; }
        public bool IsActive { get; set; }

        public Student() { }
    }
}
