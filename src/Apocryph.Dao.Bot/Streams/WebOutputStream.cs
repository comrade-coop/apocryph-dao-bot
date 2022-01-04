using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Hubs;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Services;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Apocryph.Dao.Bot.Streams
{
    public class WebOutputStream
    {
        private readonly IHubContext<WebOutputHub> _hubContext;
        private readonly IWebOutboundMessageHandler _webOutboundMessageHandler;

        public WebOutputStream(IHubContext<WebOutputHub> hubContext, IWebOutboundMessageHandler webOutboundMessageHandler)
        {
            _hubContext = hubContext;
            _webOutboundMessageHandler = webOutboundMessageHandler;
            
            _webOutboundMessageHandler.SetHandlerAction((context, message) =>
            {
                if (message != null)
                {
                    var method = !message.Errors.Any() ? "onSuccess" : "onError";
               
                    context
                        .Clients.Group(message.Session)
                        .SendAsync(method, message)
                        .GetAwaiter()
                        .GetResult();
                }
            });
        }

        public async Task RunAsync(IAsyncEnumerable<IWebOutboundMessage> messages)
        {
            await foreach (var message in messages)
            {
                try
                {
                    _webOutboundMessageHandler.Handle(_hubContext, message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to process {@Message}", message);
                }
            }
        }
    }
}