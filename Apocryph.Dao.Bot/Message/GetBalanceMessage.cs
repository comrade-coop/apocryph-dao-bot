namespace Apocryph.Dao.Bot.Message
{
    public record GetBalanceMessage(ulong UserId) : IInboundMessage
    {
    }
}