using System;
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
using System.IO;
using System.Reflection;
using System.Threading.Channels;
using Apocryph.Dao.Bot.Hubs;
using Apocryph.Dao.Bot.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Nethereum.Signer;
using Nethereum.StandardTokenEIP20;
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

            
            services.AddSingleton(new StandardTokenService(new Web3(Configuration["Ethereum:Web3Url"])
            {
                TransactionManager = { UseLegacyAsDefault = false }
            }, Configuration["Ethereum:TokenAddress"]));
            
            services.AddSingleton(Configuration);
            services.AddSingleton(Channel.CreateUnbounded<IWebInboundMessage>());
            services.AddSingleton(Channel.CreateUnbounded<IInboundMessage>());
            services.AddSingleton(Channel.CreateUnbounded<IOutboundMessage>());
            services.AddSingleton<EthereumMessageSigner>();
            
            services.AddSingleton(new DiscordSocketConfig());
            services.AddHostedService<DiscordProxyHostedService>();

            services.AddHostedService(serviceProvider => new PerperHostedService(serviceProvider, "apocryph-dao-bot"));
            services.AddControllers();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });

            services.AddSignalR();
            
            ConfigureSwagger(services, new[] {new OpenApiInfo {Title = "Api", Version = "v1"}});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(x => x.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
                c.RoutePrefix = string.Empty;
            });
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WebOutputHub>("/ws");
            });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = env.IsDevelopment() ? "ClientApp/" : "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });
             
        }
        
        public void ConfigureSwagger(IServiceCollection services, OpenApiInfo[] apiVersions)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerHeaderFilter>();
                c.SchemaFilter<EnumSchemaFilter>();

                foreach (var apiVersion in apiVersions)
                    c.SwaggerDoc(apiVersion.Version,
                        new OpenApiInfo {Title = apiVersion.Title, Version = apiVersion.Version});

                var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
