namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public record WhatsAppStudentOption(int StudentId, string Title, string Last4IdentityDigits);

    public record WhatsAppConversationState(
        int CompanyId,
        string PhoneNumberId,
        string AccessToken,
        string WaId,
        List<WhatsAppStudentOption>? PendingCandidates,
        int? StudentId,
        string? StudentDisplayName,
        string? MediaId);
}
