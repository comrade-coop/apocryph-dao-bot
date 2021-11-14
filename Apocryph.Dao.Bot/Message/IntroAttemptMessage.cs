namespace Apocryph.Dao.Bot.Message
{
    public record IntroAttemptMessage(string Session, string Address, string SignedAddress) : IWebInboundMessage;
}
