namespace Buslogix.Features.MessageExtraction.Configuration
{
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
