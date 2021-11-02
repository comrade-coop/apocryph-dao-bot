using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Perper.Application;

namespace Apocryph.Dao.Bot.Core.Services
{
    public sealed class PerperHostedService : IHostedService
    {
        private readonly string _agentName;
        private readonly IServiceProvider _serviceProvider;
        
        public PerperHostedService(IServiceProvider serviceProvider, string agentName)
        {
            _agentName = agentName;
            _serviceProvider = serviceProvider;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            PerperStartup.ServiceProvider = _serviceProvider;
            
            await PerperStartup.RunAsync($"{_agentName}-{Guid.NewGuid()}", cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}