using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Scenarios
{
    public class UserOnBoardingScenario : PerperFixture
    {
        private static string _session;
        private string _address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string _signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        private string _userName = "TestUser";
        private ulong _userId = 1000L;
        
        [Test, Order(1)]
        public async Task Processing_IntroInquiryMessage_Returns_IntroChallengeMessage()
        {
            await SendMessage<IInboundMessage>(new IntroInquiryMessage(_userName, _userId, _address));
            await ReceiveMessage<IOutboundMessage, IntroChallengeMessage>(message =>
            {
                _session = message.Session;

                message.Address.Should().Be(_address);
                message.Errors.Length.Should().Be(0);
                message.Session.Should().NotBeEmpty();
                message.UserId.Should().Be(_userId);
                message.UserName.Should().Be(_userName);
                message.UrlTemplate.Should().Contain("http");
            });
        }
        
        [Test, Order(2)]
        public async Task Processing_IntroAttemptMessage_Returns_IntroConfirmationMessage()
        {
            await SendMessage<IWebInboundMessage>(new IntroAttemptMessage(_session, _address, _signature));
            await ReceiveMessage<IOutboundMessage, IntroConfirmationMessage>(message =>
            {
                message.UserId.Should().Be(_userId);
                message.UserName.Should().Be(_userName);
                message.Session.Should().Be(_session);
                message.Errors.Length.Should().Be(0);
            });
        }
    }
}