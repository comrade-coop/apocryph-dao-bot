using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public interface IOutboundMessage
    {
        ulong UserId { get; init; }
        string[] Errors { get; init; }
        string DisplayOutput();
        public string ErrorsStr => string.Join(", ", Errors);
    }
}
