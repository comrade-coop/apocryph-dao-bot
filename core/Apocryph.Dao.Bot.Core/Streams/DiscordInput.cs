using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;
using Apocryph.Dao.Bot.Core.Data.Message;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordInput
    {
        private readonly IContext context;

        public DiscordInput(IContext context) => this.context = context;

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            yield return new IntroAttemptMessage();
            throw new NotImplementedException();
        }
    }
}
