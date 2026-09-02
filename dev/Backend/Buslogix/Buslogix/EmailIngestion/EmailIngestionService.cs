using Buslogix.EmailIngestion.Abstractions;
using Buslogix.Interfaces;
using Buslogix.MessageExtraction.Abstractions;
using Buslogix.Models;
using Buslogix.Models.DTO;
using static Buslogix.Utilities.Enums;

namespace Buslogix.EmailIngestion
{
    public class EmailIngestionService(
        IEmailAccountRepository emailAccountRepository,
        IEmailSenderRepository emailSenderRepository,
        IEmailClient emailClient,
        IMessageExtractionService messageExtractionService,
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
                ExtractedData? extracted = await messageExtractionService.ExtractAsync(text, ct);
                if (extracted != null)
                {
                    result.MessagesExtracted++;
                    await logHandler.WriteLog(
                        $"Email ingestion: extracted Amount={extracted.Amount}, Reference={extracted.Reference}, Date={extracted.Date} for company {account.CompanyId}.",
                        LogType.Info);
                }
                else
                {
                    result.MessagesUnmatched++;
                    await logHandler.WriteLog(
                        $"Email ingestion: no extraction match for company {account.CompanyId}, account {account.EmailAddress}.",
                        LogType.Warning);
                }
                return true;
            }
            catch (Exception ex)
            {
                await logHandler.WriteLog(
                    $"Email ingestion: error extracting message for company {account.CompanyId}, account {account.EmailAddress}: {ex.Message}",
                    LogType.Error);
                return false;
            }
        }
    }
}
