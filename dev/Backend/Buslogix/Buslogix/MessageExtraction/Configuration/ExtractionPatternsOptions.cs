namespace Buslogix.MessageExtraction.Configuration
{
    /// <summary>
    /// Bound from the "ExtractionPatterns" section of extraction-patterns.json.
    /// </summary>
    public class ExtractionPatternsOptions
    {
        public List<PatternDefinition> Patterns { get; set; } = [];
    }

    public class PatternDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Regex { get; set; } = string.Empty;
        public string AmountGroup { get; set; } = string.Empty;
        public string ReferenceGroup { get; set; } = string.Empty;
        public string? DateGroup { get; set; }
    }
}
