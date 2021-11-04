using System;
using System.Linq;

namespace Apocryph.Dao.Bot.Core.Message
{
    public interface IOutboundMessage
    {
        string Save();
        string[] Errors { get; }
        bool IsValid();
    }
}
