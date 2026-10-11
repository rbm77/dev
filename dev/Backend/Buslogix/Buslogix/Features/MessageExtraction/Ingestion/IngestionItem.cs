using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.MessageExtraction.Ingestion
{
    public record IngestionItem(IngestionSource Source, int CompanyId, string RawText, DateTime ReceivedAt) : ICompanyScoped;
}
