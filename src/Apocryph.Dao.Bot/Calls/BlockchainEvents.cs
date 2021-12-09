using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Streams;
using Perper.Model;
// ReSharper disable CoVariantArrayConversion

namespace Apocryph.Dao.Bot.Calls
{
    public class BlockchainEvents
    {
        private readonly IContext _context;
        private readonly Channel<IOutboundMessage> _channel;
        
        public BlockchainEvents(IContext context, Channel<IOutboundMessage> channel)
        {
            _context = context;
            _channel = channel;
        }
 
        public async Task RunAsync()
        {
            var proposalEventDtoStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(EthereumEventStream<ProposalEventDTO>), Array.Empty<object>()).ConfigureAwait(false);
            var transferEventDtoStream = await _context.StreamFunctionAsync<TransferEventDTO>(nameof(EthereumEventStream<TransferEventDTO>), Array.Empty<object>()).ConfigureAwait(false);

            var eventStreamMapperStream = await _context
                .StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper),
                    new object[] { proposalEventDtoStream, transferEventDtoStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { eventStreamMapperStream });
        }
    }
}