using System;
using System.Reflection;
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
        private readonly Assembly _assembly;
        private Task _task;
        
        public PerperHostedService(IServiceProvider serviceProvider, string agentName, Assembly assembly)
        {
            _agentName = agentName;
            _serviceProvider = serviceProvider;
            _assembly = assembly;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            PerperStartup.ServiceProvider = _serviceProvider;
            _task = PerperStartup.RunAsync($"{_agentName}", PerperStartup.DiscoverStreamAndCallTypes(_assembly), cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_task.Status == TaskStatus.Running)
            {
                _task.Dispose();    
            }
            
            await Task.CompletedTask;
        }
    }
}