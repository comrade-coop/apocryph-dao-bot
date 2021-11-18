using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;

namespace Apocryph.Dao.Bot.Streams
{
    public class DiscordOutputStream
    {
        private readonly Channel<IOutboundMessage> _channel;

        public DiscordOutputStream(Channel<IOutboundMessage> channel)
        {
            _channel = channel;
        }

        public async Task RunAsync(IAsyncEnumerable<IOutboundMessage> introInquiryDialogStream, IAsyncEnumerable<IOutboundMessage> introAttemptDialogStream, IAsyncEnumerable<IOutboundMessage> getBalanceDialogStream)
        {
            await foreach (var message in AsyncEnumerableEx.Merge(introInquiryDialogStream, introAttemptDialogStream, getBalanceDialogStream))
            {
                await _channel.Writer.WriteAsync(message, CancellationToken.None);
            }
        }
    }
}
