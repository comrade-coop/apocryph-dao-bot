using System.Threading.Channels;
using Apocryph.Dao.Bot.Core.Message;
using Apocryph.Dao.Bot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureServices((context, services) =>
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        services.AddOptions()
            .Configure<Apocryph.Dao.Bot.Core.Configuration.Discord>(context.Configuration.GetSection("Discord"));

        services.AddOptions()
            .Configure<Apocryph.Dao.Bot.Core.Configuration.Dao>(context.Configuration.GetSection("Dao"));

        services.AddSingleton<IWeb3>(new Web3(context.Configuration["Ethereum:Web3Url"])
        {
            TransactionManager = { UseLegacyAsDefault = false }
        });
        
        services.AddSingleton(context.Configuration);
        services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
        services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());
        
        services.AddSingleton(new DiscordSocketConfig());
        services.AddHostedService<DiscordProxyHostedService>();
        
        services.AddHostedService(_ => new PerperHostedService(services.BuildServiceProvider(), "apocryph-dao-bot"));
    })
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
    })
    .Build();

await host.RunAsync();