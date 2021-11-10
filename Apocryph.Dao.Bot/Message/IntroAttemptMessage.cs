using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot.Message
{
    public record IntroAttemptMessage(string Session, string Address, string SignedAddress) : IWebInboundMessage
    {
        public void Load(string from, string message)
        {
        }
        
        public Task<bool> ValidateSession(IState state)
        {
            return state.IsValidSession(Session);
        }
    }
}
