using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public class GetBalanceValidator : AbstractValidator<GetBalanceMessage>
    {
        public GetBalanceValidator(IState state)
        {
            RuleFor(x => x.UserId).ValidateUserOnBoarding(state);
        }
    }
}