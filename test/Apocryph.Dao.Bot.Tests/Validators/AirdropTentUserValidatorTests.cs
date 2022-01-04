using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using Apocryph.Dao.Bot.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Validators
{
    [TestFixture]
    public class AirdropTentUserValidatorTests : ValidatorFixture
    {
        private InMemoryState _state;
        private AirdropTentUserValidator _validator;
        
        [SetUp]
        public async Task Setup()
        {
            _state = new InMemoryState();
            
            await _state.RegisterAddress(100L, "address");
           

            _validator = new AirdropTentUserValidator(_state, TokenService, new OptionsWrapper<Airdrop>(
                new Airdrop
                {
                    Tent = new Tent
                    {
                        Amount = 10,
                        SourceAddress = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333"
                    }
                }));
        }
        
        [Test]
        public void Returns_Error_When_User_Not_Exists_In_Tent_Server()
        {
            // arrange & act
            var result = _validator.TestValidate(new AirdropTentUserMessage(100L, false));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.UserExistsInTentServer);
            result.Errors.Count.Should().Be(3);
        }
        
        [Test]
        public void Returns_Error_When_User_Has_Participated_Already()
        {
            // arrange & act
            var result = _validator.TestValidate(new AirdropTentUserMessage(100L, true));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.UserId);
            result.Errors.Count.Should().Be(2);
        }
        
        [Test]
        public async Task Returns_Error_When_Airdrop_Account_Is_Empty()
        {
            // arrange & act
            await _state.SignAddress(100L, "address");
            var result = _validator.TestValidate(new AirdropTentUserMessage(100L, true));
            
            // assert
            result.ShouldNotHaveValidationErrorFor(person => person.UserId);
            result.ShouldHaveValidationErrorFor("Balance");
        }
    }
}