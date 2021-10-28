using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordInput
    {
        private readonly IContext context;

        public DiscordInput(IContext context) => this.context = context;

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
