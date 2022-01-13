using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Scenarios
{
    public class AirdropTentUserScenario : UserBootStrappedFixture
    {
        [Test, Ignore("resume after fixing token issue")]
        public async Task Processing_Airdrop_Returns_AirdropConfirmationMessage()
        {
            await SendMessage<IInboundMessage>(new AirdropTentUserMessage(UserId, true));
            await ReceiveMessage<IOutboundMessage, AirdropConfirmationMessage>(message =>
            {
                message.UserId.Should().Be(UserId);
                message.Errors.Length.Should().Be(0);
                message.Amount.Should().BeGreaterOrEqualTo(GetDaoBotConfig.TentAirdrop.MinAmount);
            });
        }
    }
}