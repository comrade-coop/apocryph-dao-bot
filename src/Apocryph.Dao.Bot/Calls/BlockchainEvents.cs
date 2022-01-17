using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
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
        private readonly DaoBotConfig _config;

        public BlockchainEvents(IContext context, IOptions<DaoBotConfig> options)
        {
            _context = context;
            _config = options.Value;
        }
 
        public async Task RunAsync()
        {
            // for unit / integration testing
            if (_config.SkipBlockchainEvents)
                return;
            
            var communityProposalEventStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(ProposalEventStream), new[] { _config.GetDaoVotingAddress(DaoBotConfigConst.CommunityDao) }).ConfigureAwait(false);
            //var cooperativeProposalEventStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(ProposalEventStream), new[] { _config.GetDaoVotingAddress(DaoBotConfigConst.CooperativeDao) }).ConfigureAwait(false);
            var coreTeamProposalEventStream = await _context.StreamFunctionAsync<ProposalEventDTO>(nameof(ProposalEventStream), new[] { _config.GetDaoVotingAddress(DaoBotConfigConst.CoreTeamDao) }).ConfigureAwait(false);
           
            var communityProposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { communityProposalEventStream }).ConfigureAwait(false);
            //var cooperativeProposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { cooperativeProposalEventStream }).ConfigureAwait(false);
            var coreTeamProposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EthereumEventStreamMapper), new object[] { coreTeamProposalEventStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { communityProposalEventStreamMapper });
            //await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { cooperativeProposalEventStreamMapper });
            //await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { coreTeamProposalEventStreamMapper });
        }
    }
}