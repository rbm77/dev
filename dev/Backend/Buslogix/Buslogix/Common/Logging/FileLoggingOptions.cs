namespace Buslogix.Common.Logging
{
    public class FileLoggingOptions
    {
        public string Path { get; set; } = "Logs/buslogix-.log";

        public int RetainedFileCount { get; set; } = 30;
    }
}
