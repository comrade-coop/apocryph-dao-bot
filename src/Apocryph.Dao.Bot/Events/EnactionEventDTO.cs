using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Apocryph.Dao.Bot.Events
{
    [Event("Enaction")]
    public class EnactionEventDTO : IEventDTO
    {
        [Parameter("bytes32", "voteId", 1, true)]
        public byte[] VoteId { get; set; }
        
        [Parameter("bytes32", "rationale", 2, false)]
        public byte[] Rationale { get; set; }
        
        [Parameter("bytes32", "actionsHash", 3, false)]
        public byte[] ActionsHash { get; set; }
    }
}