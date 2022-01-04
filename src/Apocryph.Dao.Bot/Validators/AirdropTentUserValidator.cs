using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Microsoft.Extensions.Options;
using Nethereum.StandardTokenEIP20;
using Nethereum.Util;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public class AirdropTentUserValidator : AbstractValidator<AirdropTentUserMessage>
    {
        public AirdropTentUserValidator(IState state, StandardTokenService tokenService, IOptions<Airdrop> options)
        {
            RuleFor(x => x.UserExistsInTentServer).Equal(true).WithMessage(ValidationResources.AirdropTentUserMessageValidator_UserExistsInTentServer);
            RuleFor(x => x.UserId)
                .ValidateUserOnBoarding(state)
                .CustomAsync( async (userId, context, _) =>
                {
                    var result = await state.GetUserAirdrop(userId, "tent");
                    if (result)
                    {
                        context.AddFailure("UserId", ValidationResources.AirdropTentUserMessageValidator_UserHasParticipatedAlready);
                    }
                });

            RuleFor(x => x).CustomAsync(async (_, context, _) =>
            {
                var senderBalance = await tokenService.BalanceOfQueryAsync(options.Value.Tent.SourceAddress);
                if (UnitConversion.Convert.FromWei(senderBalance) < options.Value.Tent.Amount)
                {
                    context.AddFailure("Balance", ValidationResources.AirdropTentUserMessageValidator_AirdropAccountIsEmpty);
                }
            });
        }
    }
}