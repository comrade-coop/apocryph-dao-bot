using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Apocryph.Dao.Bot.Core.Data
{
    [Event("Transfer")]
    public class TransferEventDTO : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "to", 2, true)]
        public string To { get; set; }

        [Parameter("uint256", "value", 3, false)]
        public BigInteger Value { get; set; }
    }
}
