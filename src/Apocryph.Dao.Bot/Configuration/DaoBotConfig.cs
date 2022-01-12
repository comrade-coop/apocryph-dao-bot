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

        public string GetDaoVotingAddress(string name) => DaoVotingAddresses.SingleOrDefault(x => x.Key == name).Value;

        public string GetDaoVotingName(string address) => DaoVotingAddresses.Where(x =>
                String.Equals(x.Value, address, StringComparison.CurrentCultureIgnoreCase))
            .Select(x => x.Key)
            .SingleOrDefault();
        
        public const string CommunityDao = "community";
        public const string CooperativeDao = "cooperative";
        public const string CoreTeamDao = "core-team";
    }
}