using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Streams;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
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
            
            var communityProposalEventStream = await _context.StreamFunctionAsync<EventLog<ProposalEventDTO>>(nameof(ProposalEventStream), new[] { _config.GetDaoVotingAddress(DaoBotConfigConst.CommunityDao) }).ConfigureAwait(false);
            var communityEnactionEventStream = await _context.StreamFunctionAsync< EventLog<EnactionEventDTO>>(nameof(EnactionEventStream), new[] { _config.GetDaoVotingAddress(DaoBotConfigConst.CommunityDao) }).ConfigureAwait(false);
           
            var communityProposalEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(ProposalEventStreamMapper), new object[] { communityProposalEventStream }).ConfigureAwait(false);
            var communityEnactionEventStreamMapper = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(EnactionEventStreamMapper), new object[] { communityEnactionEventStream }).ConfigureAwait(false);
            
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { communityProposalEventStreamMapper });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { communityEnactionEventStreamMapper });
        }
    }
}