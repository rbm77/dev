namespace Buslogix.Features.WhatsAppBot.Abstractions
{
    public interface IWhatsAppGraphClient
    {
        Task SendTextMessage(string accessToken, string phoneNumberId, string toWaId, string text, CancellationToken ct);

        Task SendButtonsMessage(string accessToken, string phoneNumberId, string toWaId, string bodyText, IReadOnlyList<(string Id, string Title)> buttons, CancellationToken ct);

        Task SendListMessage(string accessToken, string phoneNumberId, string toWaId, string bodyText, string menuLabel, IReadOnlyList<(string Id, string Title, string? Description)> rows, CancellationToken ct);

        Task<(byte[] Bytes, string MimeType)> DownloadMedia(string accessToken, string mediaId, CancellationToken ct);
    }
}
