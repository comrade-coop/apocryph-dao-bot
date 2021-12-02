using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public class GetBalanceMessageValidator : AbstractValidator<GetBalanceMessage>
    {
        public GetBalanceMessageValidator(IState state)
        {
            RuleFor(x => x.UserId).ValidateUserOnBoarding(state);
        }
    }
}