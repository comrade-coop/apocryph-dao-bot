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
        private readonly AirdropTentUserValidator _validator;
        private readonly StandardTokenService _tokenService;
        private readonly DaoBotConfig _config;

        public AirdropTentUserStream(IState state, StandardTokenService tokenService, IOptions<DaoBotConfig> options) : base(state)
        {
            _config = options.Value;
            _tokenService = tokenService;
            _validator = new AirdropTentUserValidator(state, tokenService, _config);
        }
        
        protected override async Task<AirdropConfirmationMessage> RunImplAsync(AirdropTentUserMessage message)
        {
            var result = await _validator.ValidateAsync(message);
                    
            if (result.IsValid)
            {
                var userAddress = await State.GetAddress(message.UserId);
                var txHash = await _tokenService.TransferFromRequestAsync(_config.TentAirdrop.TentTokenAddress, userAddress, UnitConversion.Convert.ToWei(_config.TentAirdrop.MinAmount));

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
                    Amount: _config.TentAirdrop.MinAmount,
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