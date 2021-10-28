using Apocryph.Dao.Bot.Core.Data;
using Apocryph.Dao.Bot.Core.Message;
using Nethereum.Parity;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class DiscordOutput
    {
        private readonly IContext context;

        public DiscordOutput(IContext context) => this.context = context;

        public async Task RunAsync(IAsyncEnumerable<IOutboundMessage> messages)
        {
            throw new NotImplementedException();
        }
    }
}
