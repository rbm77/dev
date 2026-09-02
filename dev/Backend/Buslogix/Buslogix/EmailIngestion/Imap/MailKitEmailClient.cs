using Buslogix.EmailIngestion.Abstractions;
using Buslogix.Interfaces;
using Buslogix.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using static Buslogix.Utilities.Enums;

namespace Buslogix.EmailIngestion.Imap
{
    public class MailKitEmailClient(IEmailBodyTextExtractor bodyTextExtractor, ILogHandler logHandler) : IEmailClient
    {
        private const string ProcessedLabel = "Buslogix/Processed";

        public async Task<int> ProcessNewMessagesAsync(
            EmailAccount account,
            IReadOnlyList<string> senderAddresses,
            Func<string, Task<bool>> handleMessageText,
            CancellationToken ct = default)
        {
            if (senderAddresses.Count == 0)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(account.ImapHost) || string.IsNullOrWhiteSpace(account.EmailAddress) || string.IsNullOrWhiteSpace(account.AppPassword))
            {
                throw new InvalidOperationException($"Email account {account.CompanyId}/{account.Id} is missing host, address, or password.");
            }

            using ImapClient client = new();
            await client.ConnectAsync(account.ImapHost, account.ImapPort, SecureSocketOptions.SslOnConnect, ct);
            await client.AuthenticateAsync(account.EmailAddress, account.AppPassword, ct);

            IMailFolder inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

            // Server-side filtering: only candidates from an allowed sender, not
            // already labeled as processed, within a recent window (performance -
            // correctness comes entirely from the label exclusion, not the window).
            // First-ever check for an account starts the window at today, not
            // an arbitrary number of days back.
            string senderClause = string.Join(" OR ", senderAddresses.Select(address => $"from:{address}"));
            DateTime windowStart = account.LastCheckedAt?.AddHours(-1) ?? DateTime.UtcNow.Date;
            string rawQuery = $"({senderClause}) -label:\"{ProcessedLabel}\" after:{windowStart:yyyy/MM/dd}";

            IList<UniqueId> uids = await inbox.SearchAsync(SearchQuery.GMailRawSearch(rawQuery), ct);

            foreach (UniqueId uid in uids)
            {
                try
                {
                    MimeMessage message = await inbox.GetMessageAsync(uid, ct);
                    string text = bodyTextExtractor.ExtractPlainText(message);

                    bool markAsProcessed = await handleMessageText(text);
                    if (markAsProcessed)
                    {
                        await inbox.AddLabelsAsync(uid, [ProcessedLabel], silent: true, ct);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // A single bad message (parsing/extraction/handling failure)
                    // must not block the rest of the batch or leave later
                    // messages in this account unprocessed. It stays unlabeled,
                    // so it will be retried on the next poll.
                    await logHandler.WriteLog(
                        $"Email ingestion: error processing message {uid} for account {account.EmailAddress} (company {account.CompanyId}): {ex.Message}",
                        LogType.Error);
                }
            }

            await client.DisconnectAsync(true, ct);
            return uids.Count;
        }
    }
}
