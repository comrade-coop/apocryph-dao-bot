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
            var web3 = new Web3(Configuration["DaoBot:EvmApiUrl"])
            {
                TransactionManager = { UseLegacyAsDefault = false }
            };
            
            TokenService = new StandardTokenService(web3, Configuration["DaoBot:TentAirdrop:TentTokenAddress"]);
        }
    }
}