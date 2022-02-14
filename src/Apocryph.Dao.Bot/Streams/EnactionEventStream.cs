using Apocryph.Dao.Bot.Events;
using Nethereum.Web3;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    public class EnactionEventStream : EthereumEventStream<EnactionEventDTO>
    {
        public EnactionEventStream(IState state, IWeb3 web3) : base(state, web3)
        {
        }
    }
}