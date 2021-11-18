using System;
using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroChallengeMessage(string Session, string UserName, ulong UserId, string Address, string UrlTemplate, string[] Errors) : IOutboundMessage
    {
        public string DisplayOutput()
        {
            if (IsValid())
            {
                var url = string.Format(UrlTemplate, Session, Address);
                return $"Hello {UserName}, please visit {url} to confirm your address";
            }
            
            return $"Hello {UserName}, We can not process your message: {String.Join(", ", Errors)}";
        }

        public bool IsValid()
        {
            return !Errors.Any();
        }
    }
}
