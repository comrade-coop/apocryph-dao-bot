using System.Collections.Generic;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
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
    
       
    public class EthereumEventStreamMapper<TEvent>  where TEvent : IEventDTO, new()
    {
        private readonly DaoBotConfig _config;

        public EthereumEventStreamMapper(IOptions<DaoBotConfig> options)
        {
            _config = options.Value;
        }
         
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<EventLog<TEvent>> eventStream)
        {
            await foreach (var message in eventStream)
            {
                if (message is EventLog<ProposalEventDTO> proposalEventLog)
                {
                    yield return new ProposalEventMessageBuilder().Build(_config, proposalEventLog);
                }
                else if (message is EventLog<EnactionEventDTO> enactionEventLog)
                {
                    yield return new EnactionEventMessageBuilder().Build(_config, enactionEventLog);
                }
                else if (message is EventLog<TransferEventDTO> transferEventLog)
                {
                    yield return new TransferEventMessage
                    {
                        Sender = transferEventLog.Event.From,
                        Receiver = transferEventLog.Event.To,
                        Amount = transferEventLog.Event.Value
                    };
                }
            }
        }
    }
}