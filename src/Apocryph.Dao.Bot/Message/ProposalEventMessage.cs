using System.Linq;
using Serilog;

namespace Apocryph.Dao.Bot.Message
{
    public record ProposalEventMessage(ulong VoteId, string Rationale) : IOutboundMessage
    {
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        public string ContractAddress { get; set; }
        public string DaoName { get; set; }
        public string Cid { get; set; }
        public string Title { get; set; }
        public string UrlTemplate { get; set; }
 
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                try
                {
                    var url = string.Format(UrlTemplate, Cid);
                    return string.Format(MessageResources.ProposalEventMessage_OnSuccess, VoteId, Title, url);
                }
                catch
                {
                    Log.Error("Failed to format message, {@ProposalEventMessage}", this);
                    return $"Vote Proposal VoteId, (ipfs hash {Rationale})";
                }
            }
            
            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}