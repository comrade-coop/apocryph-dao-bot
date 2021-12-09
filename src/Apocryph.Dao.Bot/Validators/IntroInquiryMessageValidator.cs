using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Nethereum.Web3;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
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
                        context.AddFailure("Address", ValidationResources.IntroInquiryMessageValidatorr_Address_NotValid);
                    }
                })
                .CustomAsync(async (_, context, _) =>
                {
                    if (!await state.IsAddressAvailable(context.InstanceToValidate.UserId, context.InstanceToValidate.Address))
                    {
                        context.AddFailure("Address", ValidationResources.IntroInquiryMessageValidatorr_Address_NotAvailable);
                    }

                    if (await state.IsAddressSigned(context.InstanceToValidate.UserId, context.InstanceToValidate.Address))
                    {
                        context.AddFailure("Address", ValidationResources.IntroInquiryMessageValidatorr_Address_ConfirmedAlready);
                    }
                });
        }
    }
}