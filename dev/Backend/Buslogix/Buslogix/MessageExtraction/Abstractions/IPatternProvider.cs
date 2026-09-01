using Buslogix.MessageExtraction.Configuration;

namespace Buslogix.MessageExtraction.Abstractions
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
