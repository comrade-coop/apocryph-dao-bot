using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using Apocryph.Dao.Bot.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Validators
{
    [TestFixture]
    public class GetBalanceMessageValidatorTests
    {
        private InMemoryState _state;
        private GetBalanceMessageValidator _validator;
        
        [SetUp]
        public void Setup()
        {
            _state = new InMemoryState();
            _validator = new GetBalanceMessageValidator(_state);
        }
        
        [Test]
        public void Returns_Error_When_Address_Has_Not_Been_Registered()
        {
            // arrange & act
            var result = _validator.TestValidate(new GetBalanceMessage(100L));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.UserId);
            result.Errors.Count.Should().Be(1);
        }
        
        [Test]
        public async Task Returns_Error_When_Address_Has_Not_Been_Confirmed()
        {
            // arrange
            await _state.RegisterAddress(100L, "address");
            
            // act
            var result = _validator.TestValidate(new GetBalanceMessage(100L));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.UserId);
            result.Errors.Count.Should().Be(1);
        }
    }
}