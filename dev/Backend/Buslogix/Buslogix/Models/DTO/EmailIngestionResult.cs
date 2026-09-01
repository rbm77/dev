namespace Buslogix.Models.DTO
{
    public class EmailIngestionResult
    {
        public int AccountsChecked { get; set; }
        public int MessagesFound { get; set; }
        public int MessagesExtracted { get; set; }
        public int MessagesUnmatched { get; set; }

        public EmailIngestionResult() { }
    }
}
