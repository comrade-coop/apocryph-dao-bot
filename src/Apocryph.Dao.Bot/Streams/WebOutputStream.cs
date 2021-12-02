using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Hubs;
using Apocryph.Dao.Bot.Message;
using Microsoft.AspNetCore.SignalR;

namespace Apocryph.Dao.Bot.Streams
{
    public class WebOutputStream
    {
        private readonly IHubContext<WebOutputHub> _hubContext;

        public WebOutputStream(IHubContext<WebOutputHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task RunAsync(IAsyncEnumerable<IOutboundMessage> messages)
        {
            await foreach (var message in messages)
            {
                if (message is IntroConfirmationMessage introConfirmationMessage)
                {
                    var method = !introConfirmationMessage.Errors.Any() ? "onSuccess" : "onError";
                    await _hubContext
                        .Clients.Group(introConfirmationMessage.Session)
                        .SendAsync(method, introConfirmationMessage);
                }
            }
        }
    }
}