using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Services;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.Web3;
using Serilog;
using System.Collections.Generic;
using System.Threading.Channels;
using VueCliMiddleware;

namespace Apocryph.Dao.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            services.AddOptions()
                .Configure<Configuration.Discord>(Configuration.GetSection("Discord"));

            services.AddOptions()
                .Configure<Configuration.Dao>(Configuration.GetSection("Dao"));

            services.AddSingleton<IWeb3>(new Web3(Configuration["Ethereum:Web3Url"])
            {
                TransactionManager = { UseLegacyAsDefault = false }
            });

            services.AddSingleton(Configuration);
            services.AddSingleton(new InboundMessageFactory(new Dictionary<string, IInboundMessage>()
            {
                ["confirm"] = new IntroAttemptMessage()
            }));
            services.AddSingleton(Channel.CreateUnbounded<(string, string)>());
            services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
            services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());

            services.AddSingleton(new DiscordSocketConfig());
            services.AddHostedService<DiscordProxyHostedService>();

            services.AddHostedService(serviceProvider => new PerperHostedService(serviceProvider, "apocryph-dao-bot"));

            services.AddControllers();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp/";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }

            });
        }
    }
}
