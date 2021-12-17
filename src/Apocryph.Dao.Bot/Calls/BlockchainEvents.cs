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
            var coreTeamProposalEventStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(ProposalEventStream), new[] { _options.CoreTeamVotingContractAddress }).ConfigureAwait(false);
            var proposalEventStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(ProposalEventStream), new[] { _options.ApocryphDaoVotingContractAddress }).ConfigureAwait(false);
           
            var coreTeamProposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { coreTeamProposalEventStream }).ConfigureAwait(false);
            var proposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { proposalEventStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { coreTeamProposalEventStreamMapper });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { proposalEventStreamMapper });
        }
    }
}