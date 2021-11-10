namespace Apocryph.Dao.Bot.Message
{
    public record IntroConfirmationMessage(string Session, string UserName, ulong UserId, string UrlTemplate) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            return "Confirmed";
        }
    }
}
