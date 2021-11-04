namespace Apocryph.Dao.Bot.Core.Message
{
    public record IntroInquiryMessage(string UserName, ulong UserId, string Address) : IInboundMessage;
}
