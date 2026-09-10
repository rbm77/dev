using Buslogix.Features.EmailIngestion.Abstractions;
using Buslogix.Features.MessageExtraction.Abstractions;
using Buslogix.Features.MessageExtraction.Ingestion;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.EmailIngestion
{
    public class EmailIngestionService(
        IEmailAccountRepository emailAccountRepository,
        IEmailSenderRepository emailSenderRepository,
        IEmailClient emailClient,
        IMessageIngestionQueue messageIngestionQueue,
        ILogHandler logHandler) : IEmailIngestionService
    {
        private const int AllDataPageSize = 100000;

        public async Task<EmailIngestionResult> ProcessAllAccounts(CancellationToken ct = default)
        {
            EmailIngestionResult result = new();

            PagedResult<EmailAccount> accountsPage = await emailAccountRepository.GetEmailAccounts(null, true, 1, AllDataPageSize);
            List<EmailAccount> accounts = accountsPage.Items;
            PagedResult<EmailSender> sendersPage = await emailSenderRepository.GetEmailSenders(null, true, 1, AllDataPageSize);
            List<EmailSender> senders = sendersPage.Items;

            ILookup<int, string> sendersByCompany = senders.ToLookup(
                sender => sender.CompanyId,
                sender => sender.SenderAddress ?? string.Empty);

            foreach (EmailAccount account in accounts)
            {
                try
                {
                    List<string> senderAddresses = sendersByCompany[account.CompanyId]
                        .Where(address => !string.IsNullOrWhiteSpace(address))
                        .ToList();

                    if (senderAddresses.Count == 0)
                    {
                        continue;
                    }

                    int found = await emailClient.ProcessNewMessages(account, senderAddresses,
                        text => HandleMessage(account, text, result, ct), ct);

                    result.MessagesFound += found;
                    result.AccountsChecked++;
                    await emailAccountRepository.UpdateLastChecked(account.CompanyId, account.Id);
                }
                catch (Exception ex)
                {
                    await logHandler.WriteLog(
                        $"Email ingestion: error processing account {account.EmailAddress} (company {account.CompanyId}): {ex.Message}",
                        LogType.Error);
                }
            }

            return result;
        }

        private async Task<bool> HandleMessage(EmailAccount account, string text, EmailIngestionResult result, CancellationToken ct)
        {
            try
            {
                await messageIngestionQueue.Enqueue(
                    new IngestionItem(IngestionSource.Email, account.CompanyId, text, DateTime.UtcNow), ct);

                result.MessagesQueued++;
                await logHandler.WriteLog(
                    $"Email ingestion: queued message for extraction, company {account.CompanyId}.",
                    LogType.Info);
                return true;
            }
            catch (Exception ex)
            {
                await logHandler.WriteLog(
                    $"Email ingestion: error queueing message for company {account.CompanyId}, account {account.EmailAddress}: {ex.Message}",
                    LogType.Error);
                return false;
            }
        }
    }
}
