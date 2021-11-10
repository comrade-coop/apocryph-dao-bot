using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Perper.Model;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroAttemptDialogStream
    {
        private readonly IntroAttemptMessageValidator _validator;
        private readonly IState _state;
        private readonly IOptions<Configuration.Dao> _options;

        public IntroAttemptDialogStream(IntroAttemptMessageValidator validator, IState state, IOptions<Configuration.Dao> options)
        {
            _validator = validator;
            _options = options;
            _state = state;
        }

        public async IAsyncEnumerable<IOutboundMessage> RunAsync(IAsyncEnumerable<IntroAttemptMessage> messages)
        {
            await foreach (var message in messages)
            {
                var sessionLog = Log.ForContext("Session", message.Session);
                
                var result = await _validator.ValidateAsync(message, CancellationToken.None);
                    
                if (result.IsValid)
                {
                    var webSessionData = await _state.TryGetAsync<WebSessionData>(message.Session);
                        
                    yield return new IntroConfirmationMessage(
                        Session: message.Session,
                        UserName: webSessionData.Item2.UserName,
                        UserId: webSessionData.Item2.UserId,
                        UrlTemplate: _options.Value.SignAddressUrlTemplate); 
                    
                    sessionLog.Information("Processed {@Message}", message);
                }
                else
                {
                    var errorMessage = new ErrorMessage(message.Session, result.Errors.Select(x => x.ErrorMessage).ToArray()); 
                    yield return errorMessage;
                    
                    sessionLog.Information("Processed {@ErrorMessage}", errorMessage);
                }
            }
        }
    }
}