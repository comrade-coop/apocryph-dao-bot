using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Streams;
using Microsoft.Extensions.Options;
using Perper.Model;
// ReSharper disable CoVariantArrayConversion

namespace Apocryph.Dao.Bot.Calls
{
    public class BlockchainEvents
    {
        private readonly IContext _context;
        private readonly Configuration.Dao _options;

        public BlockchainEvents(IContext context, IOptions<Configuration.Dao> options)
        {
            _context = context;
            _options = options.Value;
        }
 
        public async Task RunAsync()
        {
            var coreTeamProposalEventDtoStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(EthereumEventStream<ProposalEventDTO>), new[] { _options.CoreTeamVotingContractAddress }).ConfigureAwait(false);
            var apocryphDaoProposalEventDtoStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(EthereumEventStream<ProposalEventDTO>), new[] { _options.ApocryphDaoVotingContractAddress }).ConfigureAwait(false);

            var coreTeamProposalEventDtoStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { coreTeamProposalEventDtoStream }).ConfigureAwait(false);
            var apocryphDaoProposalEventDtoStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { apocryphDaoProposalEventDtoStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { coreTeamProposalEventDtoStreamMapper });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { apocryphDaoProposalEventDtoStreamMapper });
        }
    }
}