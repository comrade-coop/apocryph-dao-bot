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
            var generator = await context.StreamFunctionAsync<string>(nameof(EthereumTransactions), new object[] { 20 });
            await foreach (var item in generator)
            {
                Console.WriteLine($"Generated: {item}");
            }
        }
    }
}
