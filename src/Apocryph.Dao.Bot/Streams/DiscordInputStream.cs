using Apocryph.Dao.Bot.Message;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Apocryph.Dao.Bot.Streams
{
    public class DiscordInputStream
    {
        private readonly Channel<IInboundMessage> _channel;

        public DiscordInputStream(Channel<IInboundMessage> channel)
        {
            _channel = channel;
        }

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            while (await _channel.Reader.WaitToReadAsync())
            {
                yield return await _channel.Reader.ReadAsync();
            }
        }
    }
}
