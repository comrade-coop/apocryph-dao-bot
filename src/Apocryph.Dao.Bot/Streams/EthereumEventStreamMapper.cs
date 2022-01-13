using System.Collections.Generic;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Apocryph.Dao.Bot.Streams
{
    public class EthereumEventStreamMapper
    {
        private readonly DaoBotConfig _config;

        public EthereumEventStreamMapper(IOptions<DaoBotConfig> options)
        {
            _config = options.Value;
        }
    
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IEventDTO> eventStream)
        {
            await foreach (var message in eventStream)
            {
                if (message is ProposalEventDTO proposalEventDto)
                {
                    yield return new ProposalEventMessageBuilder().Build(_config, proposalEventDto.VoteId, proposalEventDto.Rationale);
                }
                
                else if (message is TransferEventDTO transferEventDto)
                {
                    yield return new TransferEventMessage
                    {
                        Sender = transferEventDto.From,
                        Receiver = transferEventDto.To,
                        Amount = transferEventDto.Value
                    };
                }
            }
        }
    }
}