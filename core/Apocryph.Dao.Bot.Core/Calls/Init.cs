using Apocryph.Dao.Bot.Core.Data;
using Apocryph.Dao.Bot.Core.Streams;
using Perper.Model;
using System;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Calls
{
    public class Init
    {
        private readonly IContext context;

        public Init(IContext context) => this.context = context;

        public async Task RunAsync()
        {
            var transactions = await context.StreamFunctionAsync<DaoTransaction>(nameof(EthereumTransactions), Array.Empty<object>());
            await foreach (var transaction in transactions)
            {
                Console.WriteLine($"Ingested: {transaction.BlockHash}");
            }
        }
    }
}
