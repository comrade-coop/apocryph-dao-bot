namespace Apocryph.Dao.Bot.Message
{
    public record ErrorMessage(string Session, string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            return string.Join(" ", Errors);
        }
    }
}