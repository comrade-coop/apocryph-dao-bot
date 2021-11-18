using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Validators;
using FluentValidation;
using Moq;
using Perper.Model;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Validators
{
    [TestFixture]
    public class GetBalanceMessageValidatorTests
    {
        [Test, Ignore("TODO")]
        public void Returns_Error_When_Address_has_not_been_registered()
        {
            var stateMock = new Mock<IState>();
            
            stateMock.Setup(x => x.TryGetAsync<string>($"user-by-userId:{100L}"))
                .Returns(() => new Task<(bool, string)>(() => new(false, string.Empty)));
            
            var validator = new GetBalanceMessageValidator(stateMock.Object);
            var result = validator.TestValidate(new GetBalanceMessage(100L));
            
            result.ShouldHaveValidationErrorFor(person => person.UserId);
            
            stateMock.VerifyAll();
        }
        
        [Test, Ignore("TODO")]
        public void Returns_Error_When_Address_has_not_been_confirmed()
        {
            var stateMock = new Mock<IState>();
            
            stateMock.Setup(x => x.TryGetAsync<string>($"user-by-userId:{100L}"))
                .Returns(() => new Task<(bool, string)>(() => new(true, "address")));

            stateMock.Setup(x => x.TryGetAsync<bool>($"user-by-userId-address:{100L}:address"))
                .Returns(() => new Task<(bool, bool)>(() => new(false, false)));
            
            var validator = new GetBalanceMessageValidator(stateMock.Object);
            var result = validator.TestValidate(new GetBalanceMessage(100L));
            
            result.ShouldHaveValidationErrorFor(person => person.UserId);
            
            stateMock.VerifyAll();
        }
    }
}