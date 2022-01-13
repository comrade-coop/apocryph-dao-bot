using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apocryph.Dao.Bot.Tests.Fixtures
{
    public abstract class ConfigurationFixture
    {
        protected IConfiguration Configuration;

        [OneTimeSetUp]
        public void ConfigurationFixtureSetup()
        {
            Configuration = new ConfigurationBuilder()
                .ConfigureTestSettings()
                .Build();
        }
    }
}