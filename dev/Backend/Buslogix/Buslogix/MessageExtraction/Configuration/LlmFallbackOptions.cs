namespace Buslogix.MessageExtraction.Configuration
{
    /// <summary>
    /// Bound from the "LlmFallback" section (extraction-patterns.json, plus
    /// user-secrets/environment-variable overrides for ApiKey - see
    /// ServiceCollectionExtensions).
    /// </summary>
    public class LlmFallbackOptions
    {
        /// <summary>
        /// Prompt template containing a {{message}} placeholder.
        /// </summary>
        public string PromptTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Claude model id used for the Tier 2 fallback. Configurable via
        /// "LlmFallback:Model" so it can be repinned without a code change;
        /// defaults to Claude Sonnet 5.
        /// </summary>
        public string Model { get; set; } = "claude-sonnet-5";
    }
}
