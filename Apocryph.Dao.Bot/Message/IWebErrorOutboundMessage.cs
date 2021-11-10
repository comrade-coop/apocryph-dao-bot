namespace Apocryph.Dao.Bot.Message
{
    public interface IWebErrorOutboundMessage : IOutboundMessage
    {
        string DisplayOutput();
        string[] Errors { get; }
        bool IsValid();
        string Session {get;init;}
    }
}