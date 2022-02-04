namespace Apocryph.Dao.Bot.Message
{
    public record ProposalEventMessage(string VoteId, string Cid) : IOutboundMessage
    {
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        public string ContractAddress { get; set; }
        public string Channel { get; set; }
        public string Title { get; set; }
        public string UrlTemplate { get; set; }
        public string Description { get; set; }

        public string DisplayOutput()
        {
            var url = string.Format(UrlTemplate, VoteId, Cid);
            return string.Format(MessageResources.ProposalEventMessage_OnSuccess, VoteId, Title, url);
        }
        
        public string GetUrl()
        {
            return string.Format(UrlTemplate, VoteId, Cid);
        }

        public string GetThumbnailUrl()
        {
            var url = string.Format(MessageResources.GetRoboHashUrl, Cid);
            return url;
        }
    }
}