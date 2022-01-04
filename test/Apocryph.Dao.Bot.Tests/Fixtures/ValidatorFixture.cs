using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Fixtures
{
    public class ValidatorFixture : ConfigurationFixture
    {
        protected StandardTokenService TokenService;
        
        [OneTimeSetUp]
        public void ValidatorFixtureSetup()
        {
            var web3 = new Web3(Configuration["Ethereum:Web3Url"])
            {
                TransactionManager = { UseLegacyAsDefault = false }
            };
            
            TokenService = new StandardTokenService(web3, Configuration["Ethereum:TokenAddress"]);
        }
    }
}