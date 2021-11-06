using System;
using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public interface IOutboundMessage
    {
        string Save();
        string[] Errors { get; }
        bool IsValid();
    }
}
