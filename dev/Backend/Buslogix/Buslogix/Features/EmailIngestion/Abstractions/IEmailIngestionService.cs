
namespace Buslogix.Features.EmailIngestion.Abstractions
{
    public interface IEmailIngestionService
    {
        Task<EmailIngestionResult> ProcessAllAccountsAsync(CancellationToken ct = default);
    }
}
