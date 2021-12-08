using System.Linq;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Microsoft.Extensions.Options;
using Nethereum.StandardTokenEIP20;
using Nethereum.Util;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    public class AirdropTentUserStream : InboundStream<AirdropTentUserMessage, AirdropConfirmationMessage>
    {
        private readonly AirdropTentUserMessageValidator _validator;
        private readonly StandardTokenService _tokenService;
        private readonly IOptions<Airdrop> _options;

        public AirdropTentUserStream(IState state, StandardTokenService tokenService, IOptions<Airdrop> options) : base(state)
        {
            _options = options;
            _tokenService = tokenService;
            _validator = new AirdropTentUserMessageValidator(state, tokenService, options);
        }
        
        protected override async Task<AirdropConfirmationMessage> RunImplAsync(AirdropTentUserMessage message)
        {
            var result = await _validator.ValidateAsync(message);
                    
            if (result.IsValid)
            {
                var userAddress = await State.GetAddress(message.UserId);
                var txHash = await _tokenService.TransferFromRequestAsync(
                    _options.Value.Tent.SourceAddress, userAddress,
                    UnitConversion.Convert.ToWei(_options.Value.Tent.Amount));

                if (string.IsNullOrEmpty(txHash))
                {
                    return new AirdropConfirmationMessage(
                        UserId: message.UserId,
                        Amount: 0,
                        TxHash: string.Empty,
                        Errors: new[] { "Failed to withdraw funds, please try later" });
                }
               
                return new AirdropConfirmationMessage(
                    UserId: message.UserId, 
                    Amount: _options.Value.Tent.Amount,
                    TxHash: txHash);
            }
          
            return new AirdropConfirmationMessage(
                UserId: message.UserId, 
                Amount: 0,
                TxHash: string.Empty,
                Errors: result.Errors.Select(x => x.ErrorMessage).ToArray());
        }
    }
}