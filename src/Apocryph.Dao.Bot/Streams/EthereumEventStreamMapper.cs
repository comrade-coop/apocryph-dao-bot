using System.Collections.Generic;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Apocryph.Dao.Bot.Streams
{
    public class EthereumEventStreamMapper
    {
        private readonly Configuration.Dao _options;

        public EthereumEventStreamMapper(IOptions<Configuration.Dao> options)
        {
            _options = options.Value;
        }
    
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IEventDTO> eventStream)
        {
            await foreach (var message in eventStream)
            {
                if (message is ProposalEventDTO proposalEventDto)
                {
                    yield return new ProposalEventMessage(proposalEventDto.VoteId, proposalEventDto.Rationale, _options.VoteProposalUrlTemplate);
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