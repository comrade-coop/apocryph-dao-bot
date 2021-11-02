//TODO: Add PerperStartup.ServiceProvider = host.Services
//TODO: Add PerperStartup as Hosted Service

//await Perper.Application.PerperStartup.RunAsync($"apocryph-dao-bot-{System.Guid.NewGuid()}", new System.Threading.CancellationToken());

using Apocryph.Dao.Bot.Core.Model;
using Apocryph.Dao.Bot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        services.AddTransient<LocalTokenState>();
        services.AddSingleton(new DiscordSocketConfig());
        services.AddHostedService<MessageListener>();
        services.AddSingleton(context.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        var s = services.BuildServiceProvider();
        var c = s.GetService(typeof(IConfiguration));
        services.AddHostedService(_ => new PerperHostedService(services.BuildServiceProvider(), "apocryph-dao-bot"));
    })
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
    })
    .Build();

await host.RunAsync();