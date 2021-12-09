namespace Apocryph.Dao.Bot.Message
{
    public record AirdropTentUserMessage(ulong UserId, bool UserExistsInTentServer) : IInboundMessage;
}