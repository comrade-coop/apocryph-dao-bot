using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public interface IOutboundMessage
    {
        ulong UserId { get; init; }
        string[] Errors { get; init; }
        bool IsValid();
        string DisplayOutput(); 
    }
}
