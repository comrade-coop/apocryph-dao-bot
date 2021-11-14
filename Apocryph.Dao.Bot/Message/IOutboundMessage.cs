namespace Apocryph.Dao.Bot.Message
{
    public interface IOutboundMessage
    {
        ulong UserId { get; init; }
        string DisplayOutput(); 
    }
}
