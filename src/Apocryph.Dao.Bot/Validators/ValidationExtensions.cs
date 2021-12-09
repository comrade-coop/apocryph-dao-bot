using FluentValidation;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public static class ValidationExtensions 
    {
        public static IRuleBuilderOptionsConditions<T, ulong> ValidateUserOnBoarding<T>(this IRuleBuilder<T, ulong> ruleBuilder, IState state) 
        {
            return ruleBuilder.CustomAsync(async (userId, context, cancellationToken) =>
            {
                var result = await state.TryGetAsync<string>(StateExtensions.UserByUserId(userId));
                if (!result.Item1)
                {
                    context.AddFailure("UserId", ValidationResources.ValidateUserOnBoarding_UserId_AddressNotRegistered);
                }
                else
                {
                    if (!await state.IsAddressSigned(userId, result.Item2))
                    {
                        context.AddFailure("UserId", ValidationResources.ValidateUserOnBoarding_UserId_AddressNotConfirmed);
                    }
                }
            });
        }
    }
}