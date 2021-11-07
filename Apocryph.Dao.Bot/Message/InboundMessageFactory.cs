using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Message
{
    public class InboundMessageFactory
    {
        private readonly IDictionary<string, IInboundMessage> _mapping;
        public InboundMessageFactory(IDictionary<string, IInboundMessage> mapping) => _mapping = mapping;
        public IInboundMessage CreateInstance(string command) => (IInboundMessage)Activator.CreateInstance(_mapping[command].GetType());
    }
}
