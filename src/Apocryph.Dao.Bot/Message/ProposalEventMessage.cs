namespace Apocryph.Dao.Bot.Message
{
    public record ProposalEventMessage(ulong VoteId) : IOutboundMessage
    {
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        public string DisplayOutput()
        {
            throw new System.NotImplementedException();
        }
    }
}