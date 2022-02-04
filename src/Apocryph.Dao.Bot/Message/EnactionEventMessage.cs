namespace Apocryph.Dao.Bot.Message
{
    public record EnactionEventMessage(string VoteId) : IOutboundMessage
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
    }
}