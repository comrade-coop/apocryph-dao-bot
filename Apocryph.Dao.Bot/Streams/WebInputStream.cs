using Apocryph.Dao.Bot.Message;
using Perper.Model;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Apocryph.Dao.Bot.Streams
{
    public class WebInputStream
    {
        private readonly Channel<(string, string)> _channel;
        private readonly IState _state;
        private readonly InboundMessageFactory _messageFactory;

        public WebInputStream(Channel<(string, string)> channel, IState state, InboundMessageFactory messageFactory)
        {
            _channel = channel;
            _state = state;
            _messageFactory = messageFactory;
        }

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            await foreach(var (session, message) in _channel.Reader.ReadAllAsync())
            {
                if (await _state.IsValidSession(session))
                {
                    var result = _messageFactory.CreateInstance(message.GetSlashCommand());
                    result.Load(string.Empty, message);
                    yield return result;
                }
            }
        }
    }
}
