using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class IntroInquiryStreamTests : PerperFixture
    {
        private string _invalidAddress = "0x____________invalid-address______________";
        private string _userName = "TestUser";
        private ulong _userId = 1000L;
        
        [Test]
        public async Task Processing_IntroInquiryMessage_Returns_Errors()
        {
            await SendMessage<IInboundMessage>(new IntroInquiryMessage(_userName, _userId, _invalidAddress));
            await ReceiveMessage<IOutboundMessage, IntroChallengeMessage>(message =>
            {
                message.Errors.Length.Should().BeGreaterThan(0);
                message.UserId.Should().Be(_userId);
                message.UserName.Should().Be(_userName);
                message.Address.Should().Be(_invalidAddress);
                message.UrlTemplate.Should().StartWith("http");
            });
        }
    }
}