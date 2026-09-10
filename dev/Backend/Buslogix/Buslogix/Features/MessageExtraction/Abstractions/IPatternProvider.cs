using Buslogix.Features.MessageExtraction.Configuration;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    internal interface IPatternProvider
    {
        IReadOnlyList<CompiledPattern> GetPatterns();
    }
}
