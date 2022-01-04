using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Scenarios
{
    public class UserOnBoardingScenario : PerperFixture
    {
        protected static string Session;
        protected ulong UserId = 1000L;
        
        private string _address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string _signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        private string _userName = "TestUser";
        
        [Test, Order(1)]
        public async Task Processing_IntroInquiryMessage_Returns_IntroChallengeMessage()
        {
            await SendMessage<IInboundMessage>(new IntroInquiryMessage(_userName, UserId, _address));
            await ReceiveMessage<IOutboundMessage, IntroChallengeMessage>(message =>
            {
                Session = message.Session;

                message.Address.Should().Be(_address);
                message.Errors.Length.Should().Be(0);
                message.Session.Should().NotBeEmpty();
                message.UserId.Should().Be(UserId);
                message.UserName.Should().Be(_userName);
                message.UrlTemplate.Should().Contain("http");
            });
        }
        
        [Test, Order(2)]
        public async Task Processing_IntroAttemptMessage_Returns_IntroConfirmationMessage()
        {
            await SendMessage<IWebInboundMessage>(new IntroAttemptMessage(Session, _address, _signature));
            await ReceiveMessage<IOutboundMessage, IntroConfirmationMessage>(message =>
            {
                message.UserId.Should().Be(UserId);
                message.UserName.Should().Be(_userName);
                message.Session.Should().Be(Session);
                message.Errors.Length.Should().Be(0);
            });
        }
    }
}