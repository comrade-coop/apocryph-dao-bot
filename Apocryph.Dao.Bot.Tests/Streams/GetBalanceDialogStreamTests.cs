using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class GetBalanceDialogStreamTests : PerperFixture
    {
        private ulong _userId = 1000L;
        
        [Test]
        public async Task Processing_GetBalanceMessage_Returns_Errors()
        {
            await SendMessage<IInboundMessage>(new GetBalanceMessage(_userId));
            await ReceiveMessage<IOutboundMessage, BalanceMessage>(message =>
            {
                message.Errors.Length.Should().BeGreaterThan(0);
                message.UserId.Should().Be(_userId);
                message.Amount.Should().Be(0);
            });
        }
    }
}