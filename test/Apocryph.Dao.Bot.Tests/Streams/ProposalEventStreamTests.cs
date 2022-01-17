using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class ProposalEventStreamTests : PerperFixture
    {
        public override IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .ConfigureTestSettings()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "DaoBot:SkipBlockchainEvents", "false" },
                })
                .Build();
        }
 
        [Test, Ignore("additional setup required")]
        public async Task ReceiveEvents()
        {
            var count = 0;
            var channel = Host.Services.GetService<Channel<IOutboundMessage>>();
            while (await channel.Reader.WaitToReadAsync())
            {
                var message = await channel.Reader.ReadAsync();
                if (message is ProposalEventMessage _)
                {
                    count++;
                    if (count == 2)
                        break;
                }
            }
        }
    }
}