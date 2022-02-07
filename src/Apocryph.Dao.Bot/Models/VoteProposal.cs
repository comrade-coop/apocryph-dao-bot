namespace Apocryph.Dao.Bot.Models
{
    public class VoteProposal
    {
        public string ContractAddress { get; set; }
        public long ExpirationBlock { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionsHash { get; set; }
    }
}