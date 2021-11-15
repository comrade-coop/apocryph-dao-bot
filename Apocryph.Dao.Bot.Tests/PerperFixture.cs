using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using NUnit.Framework;
using Serilog;

namespace Apocryph.Dao.Bot.Tests
{
    public class PerperFixture
    {
        protected IHost Host;
 
        [OneTimeSetUp]
        public void SetupFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .ConfigureTestSettings()
                .Build();
            
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureServices((_, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

                    services.AddOptions()
                        .Configure<Configuration.Discord>(configuration.GetSection("Discord"));

                    services.AddOptions()
                        .Configure<Configuration.Dao>(configuration.GetSection("Dao"));

                    services.AddSingleton<IWeb3>(new Web3(configuration["Ethereum:Web3Url"])
                    {
                        TransactionManager = { UseLegacyAsDefault = false }
                    });
            
                    services.AddSingleton(new StandardTokenService(new Web3(configuration["Ethereum:Web3Url"])
                    {
                        TransactionManager = { UseLegacyAsDefault = false }
                    }, configuration["Ethereum:TokenAddress"]));
            
                    services.AddSingleton(configuration);
                    services.AddSingleton(Channel.CreateUnbounded<IWebInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());
                    services.AddSingleton<EthereumMessageSigner>();
                    services.AddHostedService(serviceProvider => new PerperHostedService(serviceProvider, $"apocryph-dao-bot-testing-{Guid.NewGuid()}"));
                })
                .ConfigureLogging((_, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .Build();
             
            Host.StartAsync();
        }
        
        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Host?.StopAsync(TimeSpan.FromSeconds(60));
        }
    }
}