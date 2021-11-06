using System;
using Nethereum.Signer;
using Nethereum.Web3;
using Perper.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroAttemptDialogStream
    {
        private readonly IOptions<Configuration.Dao> _options;
        private readonly IntroInquiryMessageValidator _messageValidator;
        private readonly EthereumMessageSigner _messageSigner;

        public IntroAttemptDialogStream(IOptions<Configuration.Dao> options, IState state, IWeb3 web3)
        {
            _options = options;
            
            _messageValidator = new IntroInquiryMessageValidator(state, web3);
            _messageSigner = new EthereumMessageSigner();
        }

        public async IAsyncEnumerable<IntroConfirmationMessage> RunAsync(IAsyncEnumerable<IInboundMessage> messages)
        {
            yield return new IntroConfirmationMessage();
            throw new NotImplementedException();
        }
    }
}
