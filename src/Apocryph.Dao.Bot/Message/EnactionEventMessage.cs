namespace Apocryph.Dao.Bot.Message
{
    public record EnactionEventMessage(string VoteId, string Cid) : IOutboundMessage
    {
        public string Channel { get; set; }
        public string ContractAddress { get; set; }
        public string Title { get; set; }
        public string UrlTemplate { get; set; }
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        public string DisplayOutput()
        {
            return string.Empty;
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