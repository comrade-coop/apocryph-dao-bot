using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using Apocryph.Dao.Bot.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Nethereum.Signer;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Validators
{
    [TestFixture]
    public class IntroAttemptValidatorTests
    {
        private InMemoryState _state;
        private IntroAttemptValidator _validator;
        
        private string Address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string Signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        
        [SetUp]
        public async Task Setup()
        {
            _state = new InMemoryState();
            
            await _state.RegisterAddress(100L, "address");

            _validator = new IntroAttemptValidator(new EthereumMessageSigner(), _state, new OptionsWrapper<Configuration.Dao>(
                new Configuration.Dao
                {
                    SignAddressUrlTemplate = "http://localhost:8080/sign-address/{0}/{1}"
                }));
        }
        
        [Test]
        public void Returns_Error_When_Session_Not_Found()
        {
            var session = Guid.NewGuid().ToString();
            
            // arrange & act
            var result = _validator.TestValidate(new IntroAttemptMessage(session, Address, Signature));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.Session);
            result.Errors.Count.Should().Be(1);
        }
        
        [Test]
        public void Returns_Error_When_Address_Not_Valid()
        {
            var session = Guid.NewGuid().ToString();
            
            // arrange & act
            var result = _validator.TestValidate(new IntroAttemptMessage(session, "fake address", Signature));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.Session);
            result.Errors.Count.Should().Be(2);
        }
    }
}