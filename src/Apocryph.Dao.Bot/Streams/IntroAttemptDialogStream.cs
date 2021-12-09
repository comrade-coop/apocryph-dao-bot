using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroAttemptDialogStream : InboundStream<IntroAttemptMessage, IntroConfirmationMessage>
    {
        private readonly IntroAttemptMessageValidator _validator;

        public IntroAttemptDialogStream(IState state, EthereumMessageSigner messageSigner, IOptions<Configuration.Dao> options) : base(state)
        {
            _validator = new IntroAttemptMessageValidator(messageSigner, state, options);
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