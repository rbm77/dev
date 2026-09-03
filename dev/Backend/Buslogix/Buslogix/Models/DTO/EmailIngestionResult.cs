namespace Buslogix.Models.DTO
{
    public class EmailIngestionResult
    {
        public int AccountsChecked { get; set; }
        public int MessagesFound { get; set; }
        public int MessagesQueued { get; set; }

        public EmailIngestionResult() { }
    }
}
