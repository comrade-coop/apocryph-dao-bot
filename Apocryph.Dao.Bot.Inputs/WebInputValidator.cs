using FluentValidation;

namespace Apocryph.Dao.Bot.Inputs
{
    public class WebInputValidator : AbstractValidator<WebInput>
    {
        public WebInputValidator()
        {
            RuleFor(x => x.Session).NotNull().NotEmpty();
            RuleFor(x => x.Message).NotNull().NotEmpty();
        }    
    }
}