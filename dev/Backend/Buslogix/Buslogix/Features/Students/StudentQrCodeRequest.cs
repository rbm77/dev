namespace Buslogix.Features.Students
{
    public class StudentQrCodeRequest
    {
        public List<int> StudentIds { get; set; } = [];
        public int Size { get; set; }

        public StudentQrCodeRequest() { }
    }
}
