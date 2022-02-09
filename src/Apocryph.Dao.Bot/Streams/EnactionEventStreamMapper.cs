using System.Collections.Generic;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;

namespace Apocryph.Dao.Bot.Streams
{
    public class EnactionEventStreamMapper
    {
        private readonly DaoBotConfig _config;
        
        public EnactionEventStreamMapper(IOptions<DaoBotConfig> options)
        {
            _config = options.Value;
        }
        
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<EventLog<EnactionEventDTO>> eventStream)
        {
            await foreach (var message in eventStream)
            {
                yield return new EnactionEventMessageBuilder().Build(_config, message);
            }
        }
    }
}