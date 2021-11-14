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
                if (!await state.IsAddressRegistered(context.InstanceToValidate.UserId))
                {
                    context.AddFailure("UserId", "Address has not been registered");
                }
            });
        }
    }
}