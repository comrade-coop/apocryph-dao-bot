using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Scenarios
{
    public class GetBalanceDialogStreamValidation : PerperFixture
    {
        private static string _session;
        private string _address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string _signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        private string _userName = "TestUser";
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

// TODO:
// GetBalanceDialogStream
// IntroAttemptDialogStream
// IntroChallengeMessage