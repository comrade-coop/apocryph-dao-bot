using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Streams
{
    public class IntroInquiryDialogStreamTests : PerperFixture
    {
        private static string _session = "__session__";
        private string _address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string _userName = "TestUser";
        private ulong _userId = 1000L;
        
        [Test]
        public async Task Processing_IntroInquiryMessage_Returns_Errors()
        {
            await SendMessage<IInboundMessage>(new IntroInquiryMessage(_userName, _userId, _address));
            await ReceiveMessage<IOutboundMessage, IntroChallengeMessage>(message =>
            {
                message.Errors.Length.Should().BeGreaterThan(0);
                message.UserId.Should().Be(_userId);
                message.UserName.Should().Be(_userName);
                message.Session.Should().Be(_session);
                message.Address.Should().Be(_address);
                message.UrlTemplate.Should().StartWith("http");
            });
        }
    }
}