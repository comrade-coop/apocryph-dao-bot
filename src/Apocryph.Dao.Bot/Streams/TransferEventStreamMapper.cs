using System.Collections.Generic;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;

namespace Apocryph.Dao.Bot.Streams
{
    public class TransferEventStreamMapper
    {
        private readonly DaoBotConfig _config;
        
        public TransferEventStreamMapper(IOptions<DaoBotConfig> options)
        {
            _config = options.Value;
        }
        
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<EventLog<TransferEventDTO>> eventStream)
        {
            await foreach (var message in eventStream)
            {
                yield return new TransferEventMessage
                {
                    Sender = message.Event.From,
                    Receiver = message.Event.To,
                    Amount = message.Event.Value
                };
            }
        }
    }
}