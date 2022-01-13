using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroConfirmationMessage(string Session, string UserName, ulong UserId, params string[] Errors) : IOutboundMessage, IWebOutboundMessage
    {
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                return MessageResources.IntroConfirmationMessage_OnSuccess;
            }

            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}
