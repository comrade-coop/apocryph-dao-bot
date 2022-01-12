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
        public AirdropTentUserValidator(IState state, StandardTokenService tokenService, DaoBotConfig config)
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
                var senderBalance = await tokenService.BalanceOfQueryAsync(config.TentAirdrop.TentTokenAddress);
                if (UnitConversion.Convert.FromWei(senderBalance) < config.TentAirdrop.MinAmount)
                {
                    context.AddFailure("Balance", ValidationResources.AirdropTentUserMessageValidator_AirdropAccountIsEmpty);
                }
            });
        }
    }
}