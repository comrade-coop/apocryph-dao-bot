using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Tests.Fixtures;
using Apocryph.Dao.Bot.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Moq;
using Nethereum.Signer;
using Nethereum.Web3;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Validators
{
    [TestFixture]
    public class IntroInquiryMessageValidatorTests
    {
        private InMemoryState _state;
        private IntroInquiryMessageValidator _validator;

        private string _address = "0x699608158E4B13f98ad99EAb5Ccd65d2bfc2a333";
        private string _signature = "0x6bd5a6bbd46c255dbd9ec22bf56c3dc24bf9a5aafe136d660329a67c0c87462b6f944c49b7bb0f4ac0edd8a19d87842b5141986d8c31584bf0c06a92c686d6541b";
        private string _userName = "TestUser";
        private ulong _userId = 1000L;
        
        [SetUp]
        public void Setup()
        {
            _state = new InMemoryState();
            _validator = new IntroInquiryMessageValidator( _state, new Web3());
        }
        
        [Test]
        public void Returns_Error_When_Address_NotValid()
        {
            // arrange & act
            var result = _validator.TestValidate(new IntroInquiryMessage(_userName, _userId, _address));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.Address);
            result.Errors.Count.Should().Be(1);
        }
        
        [Test]
        public async Task Returns_Error_When_Address_NotAvailable()
        {
            // arrange & act
            await _state.RegisterAddress(10L, _address);
            
            var result = _validator.TestValidate(new IntroInquiryMessage(_userName, _userId, _address));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.Address);
            result.Errors.Count.Should().Be(2);
        }
        
        [Test]
        public async Task Returns_Error_When_Address_ConfirmedAlready()
        {
            // arrange & act
            await _state.RegisterAddress(_userId, _address);
            await _state.SignAddress(_userId, _address);
            
            var result = _validator.TestValidate(new IntroInquiryMessage(_userName, _userId, _address));
            
            // assert
            result.ShouldHaveValidationErrorFor(person => person.Address);
            result.Errors.Count.Should().Be(2);
        }
    }
}