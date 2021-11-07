namespace Apocryph.Dao.Bot.Message
{
    public interface IInboundMessage
    {
        void Load(string from, string message);
    }
}
