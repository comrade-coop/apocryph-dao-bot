using System.Linq;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using Nethereum.StandardTokenEIP20;
using Nethereum.Util;
using Perper.Model;

namespace Apocryph.Dao.Bot.Streams
{
    public class GetBalanceDialogStream : InboundStream<GetBalanceMessage, BalanceMessage>
    {
        private readonly StandardTokenService _tokenService;
        private readonly GetBalanceMessageValidator _validator;

        public GetBalanceDialogStream(IState state, StandardTokenService tokenService) : base(state)
        {
            _tokenService = tokenService;
            _validator = new GetBalanceMessageValidator(state);
        }

        protected override async Task<BalanceMessage> RunImplAsync(GetBalanceMessage message)
        {
            var result = await _validator.ValidateAsync(message);
                    
            if (result.IsValid)
            {
                var address = await State.GetAddress(message.UserId);
                var senderBalance = await _tokenService.BalanceOfQueryAsync(address);
                
                return new BalanceMessage(
                    UserId: message.UserId,
                    Amount: UnitConversion.Convert.FromWei(senderBalance));
            }
           
            var errors = result.Errors.Select(x => x.ErrorMessage).ToArray();
            return  new BalanceMessage(
                UserId: message.UserId,
                Amount: default,
                Errors: errors);
        }
    }
}