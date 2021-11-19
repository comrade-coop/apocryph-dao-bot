using System.Linq;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroConfirmationMessage(string Session, string UserName, ulong UserId, params string[] Errors) : IOutboundMessage
    {
        public bool IsValid() => !Errors.Any();
        
        public string DisplayOutput()
        {
            return "Confirmed";
        }
    }
}
