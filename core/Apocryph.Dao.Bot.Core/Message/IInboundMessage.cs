using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Message
{
    public interface IInboundMessage
    {
        void Load(string from, string message);
    }
}
