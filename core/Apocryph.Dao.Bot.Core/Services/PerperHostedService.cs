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
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _task;
        
        public PerperHostedService(IServiceProvider serviceProvider, string agentName)
        {
            _agentName = agentName;
            _serviceProvider = serviceProvider;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            PerperStartup.ServiceProvider = _serviceProvider;
            _task = PerperStartup.RunAsync($"{_agentName}-{Guid.NewGuid()}", _cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await _task;
        }
    }
}