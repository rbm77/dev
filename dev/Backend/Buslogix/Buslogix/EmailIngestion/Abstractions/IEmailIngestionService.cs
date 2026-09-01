using Buslogix.Models.DTO;

namespace Buslogix.EmailIngestion.Abstractions
{
    public interface IEmailIngestionService
    {
        Task<EmailIngestionResult> ProcessAllAccountsAsync(CancellationToken ct = default);
    }
}
