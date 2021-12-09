using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Scenarios
{
    public class GettingBalanceScenario : UserBootStrappedFixture
    {
        [Test]
        public async Task Processing_GetBalanceMessage_Returns_BalanceMessage()
        {
            await SendMessage<IInboundMessage>(new GetBalanceMessage(UserId));
            await ReceiveMessage<IOutboundMessage, BalanceMessage>(message =>
            {
                message.UserId.Should().Be(UserId);
                message.Errors.Length.Should().Be(0);
                message.Amount.Should().BeGreaterOrEqualTo(0);
            });
        }
    }
}