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
                    { "Dao:ApocryphDaoVotingContractAddress", "0x411f8fa863c1750f2f2516f4d9D0792C0f93b263" },
                })
                .Build();
        }
 
        [Test]
        public async Task ReceiveEvents()
        {
            var count = 0;
            var channel = Host.Services.GetService<Channel<IOutboundMessage>>();
            while (await channel.Reader.WaitToReadAsync())
            {
                var message = await channel.Reader.ReadAsync();
                if (message is ProposalEventMessage proposalEventMessage)
                {
                    count++;
                    if (count == 2)
                        break;
                }
            }
        }
    }
}