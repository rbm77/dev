using Buslogix.Models;

namespace Buslogix.EmailIngestion.Abstractions
{
    /// <summary>
    /// Adapter over the IMAP client library (MailKit). Keeps MimeMessage/UID
    /// details internal to the Imap implementation - the orchestrator only ever
    /// deals with plain text bodies.
    /// </summary>
    public interface IEmailClient
    {
        /// <summary>
        /// Connects to the account's mailbox, searches for candidate messages
        /// from the given senders that are not yet labeled as processed, and
        /// invokes <paramref name="handleMessageText"/> once per candidate with
        /// its plain text body. A candidate is labeled as processed (so it is
        /// never fetched again) only when the callback returns true - returning
        /// false leaves it unlabeled so a transient error is retried on the next
        /// poll. Returns the number of candidates found.
        /// </summary>
        Task<int> ProcessNewMessagesAsync(
            EmailAccount account,
            IReadOnlyList<string> senderAddresses,
            Func<string, Task<bool>> handleMessageText,
            CancellationToken ct = default);
    }
}
