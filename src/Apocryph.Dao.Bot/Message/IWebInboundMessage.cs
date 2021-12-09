namespace Apocryph.Dao.Bot.Message
{
    
    public interface IWebInboundMessage : IInboundMessage
    {
        public string Session {get;init;}
    }
}