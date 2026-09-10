namespace Buslogix.Features.MessageExtraction.Configuration
{
    public class LlmFallbackOptions
    {
        public string PromptTemplate { get; set; } = string.Empty;

        public string Model { get; set; } = "claude-sonnet-5";
    }
}
