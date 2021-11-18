using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record BalanceMessage(ulong UserId, decimal Amount, params string[] Errors) : IOutboundMessage
    {
        public bool IsValid() => !Errors.Any();

        public string DisplayOutput()
        {
            return $"Balance: {Amount} CRYPTH";
        } 
    }
}