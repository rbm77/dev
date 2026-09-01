namespace Buslogix.Models.DTO
{
    public class StudentQrCodeRequest
    {
        public List<int> StudentIds { get; set; } = [];
        public int Size { get; set; }

        public StudentQrCodeRequest() { }
    }
}
