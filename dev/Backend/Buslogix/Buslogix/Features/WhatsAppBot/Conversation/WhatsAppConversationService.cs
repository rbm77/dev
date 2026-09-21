using Buslogix.Features.WhatsAppBot.Abstractions;
using Buslogix.Features.WhatsAppBot.ImageExtraction;
using static Buslogix.Common.Shared.Enums;

namespace Buslogix.Features.WhatsAppBot.Conversation
{
    public class WhatsAppConversationService(
        IWhatsAppAccountRepository whatsAppAccountRepository,
        IStudentService studentService,
        IWhatsAppConversationStateStore stateStore,
        IWhatsAppImageExtractionQueue imageExtractionQueue,
        IWhatsAppGraphClient graphClient,
        ILogHandler logHandler) : IWhatsAppConversationService
    {
        private const int MaxCandidates = 10;
        private const int MaxRowTitleLength = 24;

        private const string SearchAgainId = "search_again";
        private const string ConfirmId = "confirm";
        private const string CancelId = "cancel";
        private const string ListMenuLabel = "Ver opciones";

        private const string RestartHint = "Si querés reiniciar la conversación, esperá 10 minutos sin escribirnos y volvé a empezar.";

        private const string IdentificationPromptNoPhoto = "Para identificar al estudiante, escribinos su cédula (solo números) o su nombre completo.";
        private const string IdentificationPromptWithPhoto = "Recibimos tu comprobante 📎 Para continuar, decinos la cédula (solo números) o el nombre completo del estudiante.";
        private const string NoResultsMessage = "No encontramos ningún estudiante con ese dato. ¿Podés intentar de nuevo con la cédula o el nombre completo?";
        private const string CandidateListBody = "Encontramos estos estudiantes, elegí uno 👇";
        private const string DuplicatePhotoMessage = "Ya tenemos tu comprobante — solo procesamos uno a la vez, así que vamos a usar la primera foto que enviaste 🙂";
        private const string AwaitingPhotoReminder = "Todavía necesitamos la foto del comprobante para continuar 📎";
        private const string ProcessingAck = "¡Gracias! Ya estamos procesando tu comprobante.";
        private const string CancelledMessage = "Sin problema, quedó cancelado. Escribinos cuando quieras para empezar de nuevo 😊";

        private enum ConversationStep { AwaitingIdentification, AwaitingPhoto, AwaitingFinalConfirmation }

        public async Task HandleInboundMessage(WhatsAppInboundMessage message, CancellationToken ct)
        {
            WhatsAppConversationState? state = stateStore.Get(message.WaId);

            if (state is null)
            {
                WhatsAppAccount? account = await whatsAppAccountRepository.GetWhatsAppAccountByPhoneNumberId(message.PhoneNumberId);
                if (account is null)
                {
                    await logHandler.WriteLog(
                        $"WhatsApp conversation: no active WhatsAppAccount found for phone_number_id {message.PhoneNumberId}.",
                        LogType.Warning);
                    return;
                }

                state = new WhatsAppConversationState(account.CompanyId, account.PhoneNumberId!, account.AccessToken!, message.WaId, null, null, null, null);
            }

            switch (message.Type)
            {
                case WhatsAppMessageType.Image:
                    await HandleImage(state, message.MediaId!, ct);
                    break;
                case WhatsAppMessageType.Text:
                    await HandleText(state, message.TextBody ?? string.Empty, ct);
                    break;
                case WhatsAppMessageType.Interactive:
                    await HandleInteractive(state, message.InteractiveReplyId ?? string.Empty, ct);
                    break;
            }
        }

        private async Task HandleImage(WhatsAppConversationState state, string mediaId, CancellationToken ct)
        {
            if (state.MediaId is not null)
            {
                stateStore.Set(state.WaId, state);
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, WithHint(DuplicatePhotoMessage), ct);
                return;
            }

            WhatsAppConversationState updated = state with { MediaId = mediaId };
            stateStore.Set(state.WaId, updated);

            if (updated.StudentId is null)
            {
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, IdentificationPromptWithPhoto, ct);
            }
            else
            {
                await SendFinalConfirmationSummary(updated, ct);
            }
        }

        private async Task HandleText(WhatsAppConversationState state, string text, CancellationToken ct)
        {
            ConversationStep step = DetermineStep(state);

            switch (step)
            {
                case ConversationStep.AwaitingIdentification when state.PendingCandidates is null:
                    await RunIdentificationSearch(state, text.Trim(), ct);
                    break;

                case ConversationStep.AwaitingIdentification:
                    stateStore.Set(state.WaId, state);
                    await SendCandidateList(state, state.PendingCandidates!, WithHint(CandidateListBody), ct);
                    break;

                case ConversationStep.AwaitingPhoto:
                    stateStore.Set(state.WaId, state);
                    await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, AwaitingPhotoReminder, ct);
                    break;

                default:
                    stateStore.Set(state.WaId, state);
                    await SendFinalConfirmationSummary(state, ct, appendHint: true);
                    break;
            }
        }

        private async Task HandleInteractive(WhatsAppConversationState state, string replyId, CancellationToken ct)
        {
            ConversationStep step = DetermineStep(state);

            switch (step)
            {
                case ConversationStep.AwaitingIdentification when state.PendingCandidates is not null:
                    await HandleIdentificationSelection(state, replyId, ct);
                    break;

                case ConversationStep.AwaitingIdentification:
                    stateStore.Set(state.WaId, state);
                    await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, IdentificationPromptNoPhoto, ct);
                    break;

                case ConversationStep.AwaitingPhoto:
                    stateStore.Set(state.WaId, state);
                    await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, AwaitingPhotoReminder, ct);
                    break;

                default:
                    await HandleFinalConfirmationReply(state, replyId, ct);
                    break;
            }
        }

        private async Task RunIdentificationSearch(WhatsAppConversationState state, string query, CancellationToken ct)
        {
            bool isIdentityDocument = query.Length > 0 && query.All(char.IsDigit);

            PagedResult<Student> students = await studentService.GetStudents(
                state.CompanyId,
                true,
                isIdentityDocument ? query : null,
                isIdentityDocument ? null : query,
                null,
                null,
                null,
                1,
                MaxCandidates);

            if (students.Items.Count == 0)
            {
                stateStore.Set(state.WaId, state);
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, NoResultsMessage, ct);
                return;
            }

            List<WhatsAppStudentOption> candidates = students.Items
                .Select(student => new WhatsAppStudentOption(
                    student.Id,
                    TruncateTitle($"{student.Name} {student.LastName}"),
                    Last4(student.IdentityDocument)))
                .ToList();

            WhatsAppConversationState updated = state with { PendingCandidates = candidates };
            stateStore.Set(state.WaId, updated);

            await SendCandidateList(updated, candidates, CandidateListBody, ct);
        }

        private async Task HandleIdentificationSelection(WhatsAppConversationState state, string replyId, CancellationToken ct)
        {
            if (replyId == SearchAgainId)
            {
                WhatsAppConversationState updated = state with { PendingCandidates = null };
                stateStore.Set(state.WaId, updated);
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, IdentificationPromptNoPhoto, ct);
                return;
            }

            WhatsAppStudentOption? selected = state.PendingCandidates!.FirstOrDefault(candidate => candidate.StudentId.ToString() == replyId);
            if (selected is null)
            {
                stateStore.Set(state.WaId, state);
                await SendCandidateList(state, state.PendingCandidates!, WithHint(CandidateListBody), ct);
                return;
            }

            WhatsAppConversationState confirmed = state with
            {
                StudentId = selected.StudentId,
                StudentDisplayName = selected.Title,
                PendingCandidates = null
            };
            stateStore.Set(state.WaId, confirmed);

            if (confirmed.MediaId is null)
            {
                await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId,
                    $"¡Genial! Confirmamos a {confirmed.StudentDisplayName}. Ahora envianos la foto del comprobante 📎", ct);
            }
            else
            {
                await SendFinalConfirmationSummary(confirmed, ct);
            }
        }

        private async Task HandleFinalConfirmationReply(WhatsAppConversationState state, string replyId, CancellationToken ct)
        {
            switch (replyId)
            {
                case ConfirmId:
                    stateStore.Remove(state.WaId);
                    await imageExtractionQueue.Enqueue(
                        new WhatsAppImageExtractionItem(state.CompanyId, state.PhoneNumberId, state.AccessToken, state.WaId, state.StudentId!.Value, state.MediaId!),
                        ct);
                    await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, ProcessingAck, ct);
                    break;

                case CancelId:
                    stateStore.Remove(state.WaId);
                    await graphClient.SendTextMessage(state.AccessToken, state.PhoneNumberId, state.WaId, CancelledMessage, ct);
                    break;

                default:
                    stateStore.Set(state.WaId, state);
                    await SendFinalConfirmationSummary(state, ct, appendHint: true);
                    break;
            }
        }

        private async Task SendCandidateList(WhatsAppConversationState state, List<WhatsAppStudentOption> candidates, string bodyText, CancellationToken ct)
        {
            List<(string Id, string Title, string? Description)> rows = candidates
                .Select(candidate => (candidate.StudentId.ToString(), candidate.Title, (string?)$"Cédula terminada en {candidate.Last4IdentityDigits}"))
                .ToList();
            rows.Add((SearchAgainId, "Ninguno, buscar de nuevo", null));

            await graphClient.SendListMessage(state.AccessToken, state.PhoneNumberId, state.WaId, bodyText, ListMenuLabel, rows, ct);
        }

        private async Task SendFinalConfirmationSummary(WhatsAppConversationState state, CancellationToken ct, bool appendHint = false)
        {
            string body = $"Tenemos todo listo:\n• Estudiante: {state.StudentDisplayName}\n• Comprobante: recibido ✅\nSolo procesamos uno a la vez, ¿confirmás?";
            if (appendHint)
            {
                body = WithHint(body);
            }

            await graphClient.SendButtonsMessage(state.AccessToken, state.PhoneNumberId, state.WaId, body,
                [(ConfirmId, "✅ Sí, procesar"), (CancelId, "❌ Cancelar")], ct);
        }

        private static ConversationStep DetermineStep(WhatsAppConversationState state) => state switch
        {
            { StudentId: null } => ConversationStep.AwaitingIdentification,
            { MediaId: null } => ConversationStep.AwaitingPhoto,
            _ => ConversationStep.AwaitingFinalConfirmation
        };

        private static string WithHint(string body) => $"{body}\n\n{RestartHint}";

        private static string TruncateTitle(string title) =>
            title.Length <= MaxRowTitleLength ? title : title[..(MaxRowTitleLength - 1)] + "…";

        private static string Last4(string? identityDocument)
        {
            if (string.IsNullOrEmpty(identityDocument))
            {
                return "----";
            }

            return identityDocument.Length <= 4 ? identityDocument : identityDocument[^4..];
        }
    }
}
