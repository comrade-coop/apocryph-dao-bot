using Apocryph.Dao.Bot.Core.Message;
using System.Numerics;

namespace Apocryph.Dao.Bot.Core.Data.Message
{
    public class IntroInquiryMessage : IInboundMessage
    {
        public string Username { get; set; }

        public string Address { get; set; }

        public void Load(string from, string message)
        {
            Username = from;
            Address = message;
        }
    }
}
