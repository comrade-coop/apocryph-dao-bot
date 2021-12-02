using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroChallengeMessage(string Session, string UserName, ulong UserId, string Address, string UrlTemplate, string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                var url = string.Format(UrlTemplate, Session, Address);
                return string.Format(MessageResources.IntroChallengeMessage_OnSuccess, UserName, url);
            }

            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}
