using System;
using System.Linq;

namespace Apocryph.Dao.Bot.Core.Message
{
    public record IntroChallengeMessage(string UserName, ulong UserId, string SignedMessage, string[] Errors) : IOutboundMessage
    {
        public string Save()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (IsValid())
            {
                return $"Hello {UserName}, your signed message id {SignedMessage}";
            }
            
            return $"Hello {UserName}, We can not process your message: {String.Join(", ", Errors)}";
        }

        public bool IsValid()
        {
            return !Errors.Any();
        }
    }
}
