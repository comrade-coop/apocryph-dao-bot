namespace Apocryph.Dao.Bot.Message
{
    public record BalanceMessage(ulong UserId, decimal Amount) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            return $"Balance: {Amount} CRYPTH";
        } 
    }
}