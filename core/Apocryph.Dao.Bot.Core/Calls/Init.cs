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
            var transferEvents = await context.StreamFunctionAsync<TransferEventDTO>(nameof(TransferEvents), Array.Empty<object>());
            await foreach (var transferEvent in transferEvents)
            {
                Console.WriteLine($"Ingested: event Transfer(from: {transferEvent.From}, to: {transferEvent.To}, value: {transferEvent.Value})");
            }
        }
    }
}
