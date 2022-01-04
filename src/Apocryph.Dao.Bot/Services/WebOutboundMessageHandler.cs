using System;
using Apocryph.Dao.Bot.Hubs;
using Apocryph.Dao.Bot.Message;
using Microsoft.AspNetCore.SignalR;

namespace Apocryph.Dao.Bot.Services
{
    public class WebOutboundMessageHandler : IWebOutboundMessageHandler
    {
        private Action<IHubContext<WebOutputHub>, IWebOutboundMessage> _action;

        public void Handle(IHubContext<WebOutputHub> context, IWebOutboundMessage message)
        {
            _action.Invoke(context, message);
        }

        public void SetHandlerAction(Action<IHubContext<WebOutputHub>, IWebOutboundMessage> action)
        {
            _action = action;
        }
    }
}