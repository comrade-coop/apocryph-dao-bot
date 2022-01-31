using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Calls;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using NUnit.Framework;
using Serilog;

namespace Apocryph.Dao.Bot.Tests.Fixtures
{
    public class PerperFixture
    {
        protected IHost Host;
        
        public virtual IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .ConfigureTestSettings()
                .Build();
        }
        
        [OneTimeSetUp]
        public void SetupFixture()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
            
            var configuration = GetConfiguration();
            
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureServices((_, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

                    services.AddOptions()
                        .Configure<DaoBotConfig>(configuration.GetSection("DaoBot"));
                  
                    var managedAccount = new ManagedAccount(
                        configuration["DaoBot:TentAirdrop:AccountAddress"], 
                        configuration["DaoBot:TentAirdrop:AccountPrivateKey"]);
            
                    var web3 = new Web3(managedAccount, configuration["DaoBot:EvmApiUrl"])
                    {
                        TransactionManager = { UseLegacyAsDefault = false }
                    };
            
                    services.AddSingleton<IWeb3>(web3);
                    
                    var tokenService = new StandardTokenService(web3, configuration["DaoBot:CryphTokenAddress"]);
                    var currentAllowance = tokenService.AllowanceQueryAsync(
                            configuration["DaoBot:TentAirdrop:TentTokenAddress"],
                            configuration["DaoBot:CryphTokenAddress"])
                        .GetAwaiter()
                        .GetResult();
                    
                    services.AddSingleton(tokenService);
                    
                    services.AddSingleton(configuration);
                    services.AddSingleton(Channel.CreateUnbounded<IWebInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());
                    
                    services.AddSingleton<IWebOutboundMessageHandler, WebOutboundMessageHandler>();
                    
                    services.AddSingleton<EthereumMessageSigner>();
                    services.AddHostedService(serviceProvider => new PerperHostedService(serviceProvider, $"apocryph-dao-bot-testing-{Guid.NewGuid()}", typeof(Init).Assembly));
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
        
        protected async Task SendMessage<TMessageBaseType>(TMessageBaseType message)
        {
            var inboundChannel = Host.Services.GetService<Channel<TMessageBaseType>>();
            await inboundChannel.Writer.WriteAsync(message);
            await inboundChannel.Writer.WaitToWriteAsync();
        }

        protected async Task ReceiveMessage<TChannelMessageType, TMessageBaseType>(Action<TMessageBaseType> action) where TMessageBaseType : class
        {
            var outboundChannel = Host.Services.GetService<Channel<TChannelMessageType>>();
            var msg = await outboundChannel.Reader.ReadAsync();
            var result = msg as TMessageBaseType;

            action(result);
        }

        public DaoBotConfig GetDaoBotConfig => Host.Services.GetService<DaoBotConfig>();
    }
}