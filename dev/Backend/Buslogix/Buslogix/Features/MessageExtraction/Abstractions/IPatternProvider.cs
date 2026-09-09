using Buslogix.Features.MessageExtraction.Configuration;

namespace Buslogix.Features.MessageExtraction.Abstractions
{
    /// <summary>
    /// Internal to the feature: exposes CompiledPattern (also internal), so this
    /// contract is only ever consumed within the MessageExtraction assembly - unlike
    /// IMessageExtractionService, which is the feature's public surface.
    /// </summary>
    internal interface IPatternProvider
    {
        IReadOnlyList<CompiledPattern> GetPatterns();
    }
}
