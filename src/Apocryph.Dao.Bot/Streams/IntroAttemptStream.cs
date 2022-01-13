using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroAttemptStream : InboundStream<IntroAttemptMessage, IntroConfirmationMessage>
    {
        private readonly IntroAttemptValidator _validator;

        public IntroAttemptStream(IState state, EthereumMessageSigner messageSigner) : base(state)
        {
            _validator = new IntroAttemptValidator(messageSigner, state);
        }

        protected override async Task<IntroConfirmationMessage> RunImplAsync(IntroAttemptMessage message)
        {
            var result = await _validator.ValidateAsync(message, CancellationToken.None);

            if (result.IsValid)
            {
                var webSessionData = await State.GetSession(message.Session);
                await State.SignAddress(webSessionData.UserId, message.Address);

                return new IntroConfirmationMessage(
                    Session: message.Session,
                    UserName: webSessionData.UserName,
                    UserId: webSessionData.UserId);
            }

            return new IntroConfirmationMessage(
                Session: message.Session,
                UserName: string.Empty,
                UserId: default,
                Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
        }
    }
}
