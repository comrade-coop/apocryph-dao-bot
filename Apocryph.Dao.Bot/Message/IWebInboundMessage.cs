using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot.Message
{
    
    public interface IWebInboundMessage //: IInboundMessage
    {
        public string Session {get;init;}
        Task<bool> ValidateSession(IState state);
    }
}