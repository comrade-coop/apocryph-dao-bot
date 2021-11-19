using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Apocryph.Dao.Bot.Hubs
{
    public class WebOutputHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var session = httpContext.Request.Query["session"];
            await Groups.AddToGroupAsync(Context.ConnectionId, session);
            
            await base.OnConnectedAsync();
        }
    }
}