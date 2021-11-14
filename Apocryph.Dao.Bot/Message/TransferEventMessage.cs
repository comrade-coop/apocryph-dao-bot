using System.Linq;
using System.Numerics;

namespace Apocryph.Dao.Bot.Message
{
    public class TransferEventMessage : IOutboundMessage
    {
        public string Sender { get; set; }

        public string Receiver { get; set; }

        public BigInteger Amount { get; set; }

        public ulong UserId { get; init; }

        public string DisplayOutput()
        {
            return $"{Sender} has just transfer {Amount} CRYPH to {Receiver}";
        }

        public string[] Errors { get; }
        
        public bool IsValid()
        {
            return !Errors.Any();
        }
    }
}
