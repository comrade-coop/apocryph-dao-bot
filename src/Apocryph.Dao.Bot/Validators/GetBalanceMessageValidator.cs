using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public class GetBalanceMessageValidator : AbstractValidator<GetBalanceMessage>
    {
        public GetBalanceMessageValidator(IState state)
        {
            RuleFor(x => x.UserId).CustomAsync(async (_, context, _) =>
            {
                var result = await state.TryGetAsync<string>(StateExtensions.UserByUserId(context.InstanceToValidate.UserId));

                if (!result.Item1)
                {
                    context.AddFailure("UserId", "Address has not been registered");
                }
                else
                {
                    if (!await state.IsAddressSigned(context.InstanceToValidate.UserId, result.Item2))
                    {
                        context.AddFailure("UserId", "Address has not been confirmed");
                    }
                }
            });
        }
    }
}