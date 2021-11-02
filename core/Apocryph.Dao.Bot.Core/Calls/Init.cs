using Apocryph.Dao.Bot.Core.Message;
using Apocryph.Dao.Bot.Core.Streams;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Calls
{
    public class Init
    {
        private readonly string[] dialogs = new[]
        {
            nameof(IntroInquiryDialog),
            nameof(IntroAttemptDialog)
        };

        private readonly string[] events = new[]
        {
            nameof(TransferEvents)
        };

        private readonly IContext context;

        public Init(IContext context) => this.context = context;

        public async Task RunAsync()
        {
            var discordInput = await context.StreamFunctionAsync<IInboundMessage>(nameof(DiscordInput), Array.Empty<object>());
            var outputs = await StreamBotFunctions(discordInput);
            await context.StreamActionAsync(nameof(DiscordOutput), outputs);
        }

        private async Task<IStream<IOutboundMessage>[]> StreamBotFunctions(IStream<IInboundMessage> discordInput)
        {
            var result = new List<IStream<IOutboundMessage>>();
            foreach (var d in dialogs)
            {
                result.Add(await context.StreamFunctionAsync<IOutboundMessage>(d, new object[] { discordInput }));
            }
            foreach (var e in events)
            {
                result.Add(await context.StreamFunctionAsync<IOutboundMessage>(e, Array.Empty<object>()));
            }
            return result.ToArray();
        }
    }
}