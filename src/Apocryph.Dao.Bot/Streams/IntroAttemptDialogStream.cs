using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Perper.Model;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroAttemptDialogStream
    {
        private readonly IntroAttemptMessageValidator _validator;
        private readonly IState _state;

        public IntroAttemptDialogStream(EthereumMessageSigner messageSigner, IState  state, IOptions<Configuration.Dao> options)
        {
            _validator = new IntroAttemptMessageValidator(messageSigner, state, options);
            _state = state;
        }

        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IWebInboundMessage> messages)
        {
            await foreach (var message in messages.Where(m => m is IntroAttemptMessage).Cast<IntroAttemptMessage>())
            {
                var sessionLog = Log.ForContext("Session", message.Session);
                var result = await _validator.ValidateAsync(message, CancellationToken.None);

                if (result.IsValid)
                {
                    var webSessionData = await _state.GetSession(message.Session);
                    await _state.SignAddress(webSessionData.UserId, message.Address);

                    yield return new IntroConfirmationMessage(
                        Session: message.Session,
                        UserName: webSessionData.UserName,
                        UserId: webSessionData.UserId);

                    sessionLog.Information("Processed {@Message}", message);
                }
                else
                {
                    var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
                    yield return new IntroConfirmationMessage(
                        Session: message.Session,
                        UserName: string.Empty,
                        UserId: default,
                        Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());

                    sessionLog.Information("Processed {@Message} with errors {@Errors}", message, errors);
                }
            }
        }
    }
}