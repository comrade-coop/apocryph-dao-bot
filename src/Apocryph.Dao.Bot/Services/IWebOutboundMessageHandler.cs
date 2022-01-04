using System;
using Apocryph.Dao.Bot.Hubs;
using Apocryph.Dao.Bot.Message;
using Microsoft.AspNetCore.SignalR;

namespace Apocryph.Dao.Bot.Services
{
    public interface IWebOutboundMessageHandler
    {
        void Handle(IHubContext<WebOutputHub> context, IWebOutboundMessage message);
        void SetHandlerAction(Action<IHubContext<WebOutputHub>, IWebOutboundMessage> action);
    }
}