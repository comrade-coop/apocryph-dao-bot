using System;
using Apocryph.Dao.Bot.Message;
using Perper.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroInquiryDialogStream
    {
        private readonly IState _state;
        private readonly IOptions<Configuration.Dao> _options;
        private readonly IntroInquiryMessageValidator _messageValidator;

        public IntroInquiryDialogStream(IOptions<Configuration.Dao> options, IState state, IWeb3 web3)
        {
            _options = options;
            _state = state;
            _messageValidator = new IntroInquiryMessageValidator(state, web3);
        }

        public async IAsyncEnumerable<IntroChallengeMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var attempt in messages)
            {
                if (attempt is IntroInquiryMessage attemptMessage)
                {
                    var result = await _messageValidator.ValidateAsync(attemptMessage, CancellationToken.None);
                    var session = await _state.CreateSession(Guid.NewGuid().ToString("N"), attemptMessage.UserName, attemptMessage.UserId);
                    
                    if (result.IsValid)
                    {
                        yield return new IntroChallengeMessage(
                            Session: session,
                            UserName: attemptMessage.UserName,
                            UserId: attemptMessage.UserId,
                            Address: attemptMessage.Address, 
                            UrlTemplate: _options.Value.SignAddressUrlTemplate,
                            Errors: result.Errors.Select(x => x.ErrorMessage).ToArray()); 
                    }
                    else
                    {
                        yield return new IntroChallengeMessage(
                            Session: session,
                            UserName: attemptMessage.UserName,
                            UserId: attemptMessage.UserId,
                            Address: attemptMessage.Address,
                            UrlTemplate: _options.Value.SignAddressUrlTemplate,
                            Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
                    }
                }
            }
        }
    }
}
