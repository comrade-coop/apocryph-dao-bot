using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class AirdropTentUserStreamTests : PerperFixture
    {
        private ulong _userId = 1000L;
        
        [Test, Ignore("requires additional setup?")]
        public async Task Processing_AirdropTentUser_Returns_Errors()
        {
            var outboundChannel = Host.Services.GetService<Channel<IOutboundMessage>>();
            var result = await outboundChannel.Reader.ReadAsync();
            
            await ReceiveMessage<IOutboundMessage, ProposalEventMessage>(message =>
            {
                message.UserId.Should().Be(_userId);
                message.Errors.Length.Should().Be(3);
            });
        }
    }
}