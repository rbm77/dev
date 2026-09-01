using System.Text.RegularExpressions;

namespace Buslogix.MessageExtraction.Configuration
{
    /// <summary>
    /// A pattern definition after its regex has been compiled. Never crosses the
    /// assembly boundary - internal to the MessageExtraction feature.
    /// </summary>
    internal record CompiledPattern(string Name, Regex Regex, string AmountGroup, string ReferenceGroup, string? DateGroup);
}
