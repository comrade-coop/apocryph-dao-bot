namespace Apocryph.Dao.Bot.Message
{
    public record ErrorMessage(string Session, ulong UserId, string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            return string.Join(" ", Errors);
        }
    }
}