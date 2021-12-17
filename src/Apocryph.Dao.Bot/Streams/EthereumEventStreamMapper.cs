using System.Collections.Generic;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Apocryph.Dao.Bot.Streams
{
    public class EthereumEventStreamMapper
    {
        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IEventDTO> eventStream)
        {
            await foreach (var message in eventStream)
            {
                if (message is ProposalEventDTO proposalEventDto)
                {
                    yield return new ProposalEventMessage(proposalEventDto.VoteId);
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