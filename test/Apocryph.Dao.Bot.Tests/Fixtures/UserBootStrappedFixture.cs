using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Fixtures
{
    public abstract class UserBootStrappedFixture : PerperFixture
    {
        protected static string Session;
        protected string Address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        protected string Signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        protected string UserName = "TestUser";
        protected ulong UserId = 1000L;

        [OneTimeSetUp]
        public async Task Setup()
        {
            await SendMessage<IInboundMessage>(new IntroInquiryMessage(UserName, UserId, Address));
            await ReceiveMessage<IOutboundMessage, IntroChallengeMessage>(message => { Session = message.Session; });
            await SendMessage<IWebInboundMessage>(new IntroAttemptMessage(Session, Address, Signature));
            await ReceiveMessage<IOutboundMessage, IntroConfirmationMessage>(message => { });
        }
    }
}