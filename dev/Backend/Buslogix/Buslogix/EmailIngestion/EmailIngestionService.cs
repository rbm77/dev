using Buslogix.EmailIngestion.Abstractions;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.MessageExtraction.Queue;
using Buslogix.Models;
using Buslogix.Models.DTO;
using static Buslogix.Utilities.Enums;

namespace Buslogix.EmailIngestion
{
    public class EmailIngestionService(
        IEmailAccountRepository emailAccountRepository,
        IEmailSenderRepository emailSenderRepository,
        IEmailClient emailClient,
        IMessageIngestionQueue messageIngestionQueue,
        ILogHandler logHandler) : IEmailIngestionService
    {
        private const int AllDataPageSize = 100000;

        public async Task<EmailIngestionResult> ProcessAllAccountsAsync(CancellationToken ct = default)
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

                    int found = await emailClient.ProcessNewMessagesAsync(account, senderAddresses,
                        text => HandleMessageAsync(account, text, result, ct), ct);

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

        private async Task<bool> HandleMessageAsync(EmailAccount account, string text, EmailIngestionResult result, CancellationToken ct)
        {
            try
            {
                // Hand the raw text off to the shared extraction queue and
                // return immediately - extraction itself happens later, in
                // the background, decoupled from this IMAP scan. "Processed"
                // here means "queued", not "extracted": once labeled, MailKit
                // will never surface this message again, so a failure past
                // this point is handled by MessageExtractionWorker's own
                // failure-table safety net, not by retrying the poll.
                await messageIngestionQueue.EnqueueAsync(
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
