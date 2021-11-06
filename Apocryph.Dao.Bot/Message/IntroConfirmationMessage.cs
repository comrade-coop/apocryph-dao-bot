using System;
using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroConfirmationMessage() : IOutboundMessage
    {
        public string Save()
        {
            throw new NotImplementedException();
        }

        public string[] Errors { get; }
        
        public bool IsValid()
        {
            return !Errors.Any();
        }
    }
}
