using System;
using Apocryph.Dao.Bot.Message;
using Perper.Model;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

namespace Apocryph.Dao.Bot.Streams
{
    public class IntroInquiryStream : InboundStream<IntroInquiryMessage, IntroChallengeMessage>
    {
        private readonly IState _state;
        private readonly DaoBotConfig _config;
        private readonly IntroInquiryValidator _validator;

        public IntroInquiryStream(IState state, IWeb3 web3, IOptions<DaoBotConfig> options) : base(state)
        {
            _state = state;
            _config = options.Value;
            _validator = new IntroInquiryValidator(state, web3);
        }
 
        protected override async Task<IntroChallengeMessage> RunImplAsync(IntroInquiryMessage message)
        {
            var result = await _validator.ValidateAsync(message, CancellationToken.None);
            var session = Guid.NewGuid().ToString("N");
            
            if (result.IsValid)
            {
                await _state.RegisterAddress(message.UserId, message.Address);
                await _state.CreateSession(session, message.UserName, message.UserId);
                
                return new IntroChallengeMessage(
                    Session: session,
                    UserName: message.UserName,
                    UserId: message.UserId,
                    Address: message.Address,
                    UrlTemplate: _config.SignAddressUrl,
                    Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
            }
            
            return new IntroChallengeMessage(
                Session: session,
                UserName: message.UserName,
                UserId: message.UserId,
                Address: message.Address,
                UrlTemplate: _config.SignAddressUrl,
                Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
        }
    }
}
