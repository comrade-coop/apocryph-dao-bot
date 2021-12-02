using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record AirdropConfirmationMessage(ulong UserId, decimal Amount, string TxHash, params string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                return string.Format(MessageResources.AirdropConfirmationMessage_OnSuccess, Amount, TxHash);
            }

            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}