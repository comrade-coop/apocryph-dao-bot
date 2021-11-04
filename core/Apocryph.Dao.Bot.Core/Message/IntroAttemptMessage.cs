using System;

namespace Apocryph.Dao.Bot.Core.Message
{
    public record IntroAttemptMessage(string Username, string EthereumAddress) : IInboundMessage
    {
        public void Load(string from, string message)
        {
            throw new NotImplementedException();
        }
    }
}
