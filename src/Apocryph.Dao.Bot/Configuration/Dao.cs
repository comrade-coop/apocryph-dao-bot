using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Configuration
{
    public class Dao
    {
        public string SignAddressUrlTemplate { get; set; }
        public string VoteProposalUrlTemplate { get; set; }
        public Dictionary<string, string> ContractToChannel { get; set; }
        public string CoreTeamVotingContractAddress { get; set; }
        public string ApocryphDaoVotingContractAddress { get; set; }
    }
}