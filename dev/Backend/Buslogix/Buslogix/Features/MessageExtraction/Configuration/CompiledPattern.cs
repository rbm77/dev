using System.Text.RegularExpressions;

namespace Buslogix.Features.MessageExtraction.Configuration
{
    internal record CompiledPattern(string Name, Regex Regex, string AmountGroup, string ReferenceGroup, string? DateGroup);
}
