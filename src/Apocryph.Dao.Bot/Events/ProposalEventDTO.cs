using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Apocryph.Dao.Bot.Events
{
    [Event("Proposal")]
    public class ProposalEventDTO : IEventDTO
    {
        [Parameter("uint256", "voteId", 1, true)]
        public ulong VoteId { get; set; }
        
        [Parameter("bytes32", "rationale", 2, false)]
        public byte[] Rationale { get; set; }

        public string Address { get; set; }
    }
}