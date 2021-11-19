namespace Apocryph.Dao.Bot.Message
{
    public record IntroInquiryMessage(string UserName, ulong UserId, string Address) : IInboundMessage;
}
