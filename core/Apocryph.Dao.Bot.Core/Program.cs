//TODO: Add PerperStartup.ServiceProvider = host.Services
//TODO: Add PerperStartup as Hosted Service

await Perper.Application.PerperStartup.RunAsync($"apocryph-dao-bot-{System.Guid.NewGuid()}", new System.Threading.CancellationToken());