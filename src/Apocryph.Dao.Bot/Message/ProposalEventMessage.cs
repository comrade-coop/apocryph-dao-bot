using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record ProposalEventMessage(ulong VoteId) : IOutboundMessage
    {
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                return string.Format(MessageResources.ProposalEventMessage_OnSuccess, VoteId);
            }

            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}