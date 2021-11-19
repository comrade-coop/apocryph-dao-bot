using Apocryph.Dao.Bot.Message;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Apocryph.Dao.Bot.Streams
{
    public class WebInputStream
    {
        private readonly Channel<IWebInboundMessage> _channel;

        public WebInputStream(Channel<IWebInboundMessage> channel)
        {
            _channel = channel;
        }

        public async IAsyncEnumerable<IWebInboundMessage> RunAsync()
        {
            await foreach(var message in _channel.Reader.ReadAllAsync()) 
                yield return message;
        }
    }
}