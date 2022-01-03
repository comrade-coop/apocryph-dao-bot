using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Calls;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
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
                        .Configure<Configuration.Discord>(configuration.GetSection("Discord"));

                    services.AddOptions()
                        .Configure<Configuration.Dao>(configuration.GetSection("Dao"));

                    services.AddOptions()
                        .Configure<Configuration.Airdrop>(configuration.GetSection("Airdrop"));
                    
                    var managedAccount = new ManagedAccount(
                        configuration["Airdrop:Wallet:Address"], 
                        configuration["Airdrop:Wallet:PrivateKey"]);
            
                    var web3 = new Web3(managedAccount, configuration["Ethereum:Web3Url"])
                    {
                        TransactionManager = { UseLegacyAsDefault = false }
                    };
            
                    services.AddSingleton<IWeb3>(web3);
                    
                    var tokenService = new StandardTokenService(web3, configuration["Ethereum:TokenAddress"]);
                    var currentAllowance = tokenService.AllowanceQueryAsync(
                            configuration["Airdrop:Tent:SourceAddress"],
                            configuration["Ethereum:TokenAddress"])
                        .GetAwaiter()
                        .GetResult();
                    
                    services.AddSingleton(tokenService);
                    
                    services.AddSingleton(configuration);
                    services.AddSingleton(Channel.CreateUnbounded<IWebInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
                    services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());
                    services.AddSingleton<EthereumMessageSigner>();
                    services.AddHostedService(serviceProvider => new PerperHostedService(serviceProvider, $"apocryph-dao-bot-testing-{Guid.NewGuid()}", typeof(Init).Assembly));

                    services.AddSignalR();
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
            var result = await outboundChannel.Reader.ReadAsync() as TMessageBaseType;

            action(result);
        }

        public Configuration.Discord GetDiscordConfiguration => Host.Services.GetService<Configuration.Discord>();
    }
}
