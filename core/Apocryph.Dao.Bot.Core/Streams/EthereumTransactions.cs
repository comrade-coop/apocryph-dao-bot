using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class EthereumTransactions
    {
        public async IAsyncEnumerable<string> RunAsync(int count)
        {
            for (var i = 0; i < count; i++)
            {
                await Task.Delay(100);
                yield return $"{i}. Message";
            }
        }
    }
}
