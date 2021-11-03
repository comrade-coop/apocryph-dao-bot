using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Apocryph.Dao.Bot.Core.Data.Message;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordInputStream
    {
        private readonly IContext _context;
        private readonly Channel<IInboundMessage> _channel;

        public DiscordInputStream(IContext context, Channel<IInboundMessage> channel)
        {
            _context = context;
            _channel = channel;
        }

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            while (await _channel.Reader.WaitToReadAsync())
            {
                var message = await _channel.Reader.ReadAsync();
                
                if(message is IntroAttemptMessage introAttemptMessage)
                    yield return introAttemptMessage;
                
                if(message is IntroInquiryMessage introInquiryMessage)
                    yield return introInquiryMessage;
            }
        }
    }
}
