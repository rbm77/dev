
namespace Buslogix.Features.EmailIngestion.Abstractions
{
    public interface IEmailIngestionService
    {
        Task<EmailIngestionResult> ProcessAllAccounts(CancellationToken ct = default);
    }
}
