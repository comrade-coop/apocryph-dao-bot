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
            _messageValidator = new IntroInquiryMessageValidator(state, web3);
            _options = options;
            _state = state;
        }

        public async IAsyncEnumerable<IntroChallengeMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var message in messages.Where(m => m is IntroInquiryMessage).Cast<IntroInquiryMessage>())
            {
                var result = await _messageValidator.ValidateAsync(message, CancellationToken.None);
                var session = Guid.NewGuid().ToString("N");

                await _state.RegisterAddress(message.UserId, message.Address);
                await _state.CreateSession(session, message.UserName, message.UserId);

                if (result.IsValid)
                {
                    yield return new IntroChallengeMessage(
                        Session: session,
                        UserName: message.UserName,
                        UserId: message.UserId,
                        Address: message.Address,
                        UrlTemplate: _options.Value.SignAddressUrlTemplate,
                        Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
                }
                else
                {
                    yield return new IntroChallengeMessage(
                        Session: session,
                        UserName: message.UserName,
                        UserId: message.UserId,
                        Address: message.Address,
                        UrlTemplate: _options.Value.SignAddressUrlTemplate,
                        Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
                }
            }
        }
    }
}
