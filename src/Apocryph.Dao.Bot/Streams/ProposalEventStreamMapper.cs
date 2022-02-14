using System.Collections.Generic;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;

namespace Apocryph.Dao.Bot.Streams
{
    public class ProposalEventStreamMapper
    {
        private readonly DaoBotConfig _config;
        
        public ProposalEventStreamMapper(IOptions<DaoBotConfig> options)
        {
            _config = options.Value;
        }
        
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<EventLog<ProposalEventDTO>> eventStream)
        {
            await foreach (var message in eventStream)
            {
                yield return new ProposalEventMessageBuilder().Build(_config, message);
            }
        }
    }
}