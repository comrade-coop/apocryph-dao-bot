using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordOutput
    {
        private readonly IContext _context;
        private readonly IConfiguration _configuration;

        public DiscordOutput(IContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task RunAsync(IAsyncEnumerable<IOutboundMessage> messages)
        {
            //TODO: Add respective implementation using some Discord Service
            //throw new NotImplementedException();
            await Task.CompletedTask;
        }
    }
}
