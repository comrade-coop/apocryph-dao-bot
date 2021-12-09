using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class AirdropTentUserScenario : PerperFixture
    {
        private ulong _userId = 1000L;
        
        [Test]
        public async Task Processing_AirdropTentUser_Returns_Errors()
        {
            await SendMessage<IInboundMessage>(new AirdropTentUserMessage(_userId, false));
            await ReceiveMessage<IOutboundMessage, AirdropConfirmationMessage>(message =>
            {
                message.UserId.Should().Be(_userId);
                message.Errors.Length.Should().Be(3);
                message.Amount.Should().Be(0);
            });
        }
    }
}