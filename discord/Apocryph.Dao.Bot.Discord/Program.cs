using Apocryph.Dao.Bot.Discord.Model;
using Apocryph.Dao.Bot.Discord.Services;
using Discord.WebSocket;
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
            .Configure<Apocryph.Dao.Bot.Discord.Configuration.Discord>(context.Configuration.GetSection("Discord"));

        services.AddTransient<LocalTokenState>();
        services.AddSingleton(new DiscordSocketConfig());
        services.AddHostedService<MessageListener>();
    })
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
    })
    .Build();

await host.RunAsync();