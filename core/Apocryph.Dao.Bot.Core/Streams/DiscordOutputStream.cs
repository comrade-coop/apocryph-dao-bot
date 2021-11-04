using Apocryph.Dao.Bot.Core.Message;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordOutputStream
    {
        private readonly Channel<IOutboundMessage> _channel;

        public DiscordOutputStream(Channel<IOutboundMessage> channel)
        {
            _channel = channel;
        }

        public async Task RunAsync(IAsyncEnumerable<IOutboundMessage> messages)
        {
            await foreach (var message in messages)
            {
                await _channel.Writer.WriteAsync(message, CancellationToken.None);
            }
        }
    }
}
