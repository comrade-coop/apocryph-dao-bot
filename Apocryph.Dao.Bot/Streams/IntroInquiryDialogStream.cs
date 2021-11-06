using Apocryph.Dao.Bot.Message;
using Perper.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Nethereum.Web3;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroInquiryDialogStream
    {
        private readonly IOptions<Configuration.Dao> _options;
        private readonly IntroInquiryMessageValidator _messageValidator;
        private readonly EthereumMessageSigner _messageSigner;

        public IntroInquiryDialogStream(IOptions<Configuration.Dao> options, IState state, IWeb3 web3)
        {
            _options = options;
            
            _messageValidator = new IntroInquiryMessageValidator(state, web3);
            _messageSigner = new EthereumMessageSigner();
        }

        public async IAsyncEnumerable<IntroChallengeMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            await foreach (var attempt in messages)
            {
                if (attempt is IntroInquiryMessage attemptMessage)
                {
                    var result = await _messageValidator.ValidateAsync(attemptMessage, CancellationToken.None);

                    if (result.IsValid)
                    {
                        var signedMessage = _messageSigner.EncodeUTF8AndEcRecover(attemptMessage.Address, _options.Value.MessageSignature);
                        
                        yield return new IntroChallengeMessage(
                            UserName: attemptMessage.UserName,
                            UserId: attemptMessage.UserId,
                            SignedMessage: signedMessage,
                            Errors: result.Errors.Select(x => x.ErrorMessage).ToArray()); 
                    }
                    else
                    {
                        yield return new IntroChallengeMessage(
                            UserName: attemptMessage.UserName,
                            UserId: attemptMessage.UserId,
                            SignedMessage: string.Empty,
                            Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
                    }
                }
            }
        }
    }
}
