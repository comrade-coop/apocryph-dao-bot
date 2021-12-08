using System;
using Apocryph.Dao.Bot.Message;
using FluentValidation;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using Perper.Model;

namespace Apocryph.Dao.Bot.Validators
{
    public class IntroAttemptMessageValidator  : AbstractValidator<IntroAttemptMessage>
    { 
        public IntroAttemptMessageValidator(EthereumMessageSigner messageSigner, IState state, IOptions<Configuration.Dao> options)
        {
            RuleFor(x => x.SignedAddress).NotNull().NotEmpty();
            RuleFor(x => x.Session).NotNull().NotEmpty()
                .CustomAsync(async (session, context, _) =>
                {
                    if(!await state.IsValidSession(session))
                    {
                        context.AddFailure("Session", ValidationResources.IntroAttemptMessageValidator_Session_NotFound);
                    }

                    var address = messageSigner.EncodeUTF8AndEcRecover(context.InstanceToValidate.Address, context.InstanceToValidate.SignedAddress);
                    if(!address.Equals(context.InstanceToValidate.Address, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.AddFailure("Address", ValidationResources.IntroAttemptMessageValidator_Address_NotValid);
                    }
                });
        }
    }
}