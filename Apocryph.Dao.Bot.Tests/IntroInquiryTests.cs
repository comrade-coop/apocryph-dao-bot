using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;

namespace Apocryph.Dao.Bot.Tests
{
    public class IntroInquiryTests : PerperFixture
    {
        [Test]
        public async Task Should_Success()
        {
            Log.Logger.Information("running test...");
            
            var inboundChannel = Host.Services.GetService<Channel<IInboundMessage>>();
            await inboundChannel.Writer.WriteAsync(new IntroInquiryMessage("TestUser", 1000L, "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333"));
            await inboundChannel.Writer.WaitToWriteAsync();

            var outboundChannel = Host.Services.GetService<Channel<IOutboundMessage>>();
            var result = await outboundChannel.Reader.ReadAsync();

            Assert.IsTrue(result != null);
        }
    }
}