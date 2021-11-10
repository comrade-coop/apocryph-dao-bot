namespace Apocryph.Dao.Bot.Message
{
    public record IntroInquiryMessage(string UserName, ulong UserId, string Address) : IInboundMessage
    {
        public void Load(string from, string message)
        {
            throw new System.NotImplementedException();
        }
    }
    
    
}
