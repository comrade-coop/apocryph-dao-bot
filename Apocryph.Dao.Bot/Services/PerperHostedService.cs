using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Perper.Application;

namespace Apocryph.Dao.Bot.Services
{
    public sealed class PerperHostedService : IHostedService
    {
        private readonly string _agentName;
        private readonly IServiceProvider _serviceProvider;

        private Task _task;
        
        public PerperHostedService(IServiceProvider serviceProvider, string agentName)
        {
            _agentName = agentName;
            _serviceProvider = serviceProvider;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            PerperStartup.ServiceProvider = _serviceProvider;
            //_task = PerperStartup.RunAsync($"{_agentName}", cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //_task.Dispose();
            await Task.CompletedTask;
        }
    }
}