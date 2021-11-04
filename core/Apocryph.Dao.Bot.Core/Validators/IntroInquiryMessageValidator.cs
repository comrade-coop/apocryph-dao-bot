using Apocryph.Dao.Bot.Core.Message;
using FluentValidation;
using Nethereum.Web3;
using Perper.Model;

namespace Apocryph.Dao.Bot.Core.Validators
{
    public class IntroInquiryMessageValidator : AbstractValidator<IntroInquiryMessage>
    {
        public IntroInquiryMessageValidator(IState state, IWeb3 web3)
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty();
            RuleFor(x => x.Address).NotNull().NotEmpty()
                .CustomAsync(async (address, context, _) =>
                {
                    try
                    {
                        await web3.Eth.GetBalance.SendRequestAsync(address);
                    }
                    catch
                    {
                        context.AddFailure("Address", "Address is invalid");
                    }
                }).CustomAsync(async (userName, context, _) =>
                {
                    if (await state.IsUserConfirmed(userName))
                    {
                        context.AddFailure("UserName", "Username is confirmed already");
                    }
                });
        }
    }
}