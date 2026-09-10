
namespace Buslogix.Features.EmailIngestion.Abstractions
{
    public interface IEmailClient
    {
        Task<int> ProcessNewMessages(
            EmailAccount account,
            IReadOnlyList<string> senderAddresses,
            Func<string, Task<bool>> handleMessageText,
            CancellationToken ct = default);
    }
}
