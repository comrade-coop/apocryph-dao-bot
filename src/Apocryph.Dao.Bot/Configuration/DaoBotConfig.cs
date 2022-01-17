using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocryph.Dao.Bot.Configuration
{
    public class DaoBotConfig
    {
        public string DiscordAuthToken { get; set; }
        public string SignAddressUrl { get; set; }
        public string VoteProposalUrl { get; set; }
        public Dictionary<string, string> DaoVotingAddresses { get; set; }
        public string EvmApiUrl { get; set; }
        public string CryphTokenAddress { get; set; }
        public TentAirdropConfig TentAirdrop { get; set; }
        public bool SkipBlockchainEvents { get; set; } = false;
    }

    public static class DaoBotConfigHelper
    {
        public static string GetDaoVotingAddress(this DaoBotConfig config, string name) => config.DaoVotingAddresses.SingleOrDefault(x => x.Key == name).Value;

        public static string GetDaoVotingName(this DaoBotConfig config, string address) => config.DaoVotingAddresses.Where(x =>
                String.Equals(x.Value, address, StringComparison.CurrentCultureIgnoreCase))
            .Select(x => x.Key)
            .SingleOrDefault();
    }

    public static class DaoBotConfigConst 
    {
        public const string CommunityDao = "community";
        public const string CooperativeDao = "cooperative";
        public const string CoreTeamDao = "core-team";
    }
}