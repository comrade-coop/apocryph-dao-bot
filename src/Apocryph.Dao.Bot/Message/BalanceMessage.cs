using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record BalanceMessage(ulong UserId, decimal Amount, params string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                return string.Format(MessageResources.BalanceMessage_OnSuccess, Amount);
            }

            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        } 
    }
}