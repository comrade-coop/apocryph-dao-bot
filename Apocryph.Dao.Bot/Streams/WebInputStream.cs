using Apocryph.Dao.Bot.Message;
using Perper.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace Apocryph.Dao.Bot.Streams
{
    public class WebInputStream
    {
        private readonly IState _state;
        private readonly Channel<IWebInboundMessage> _channel;

        public WebInputStream(Channel<IWebInboundMessage> channel, IState state)
        {
            _channel = channel;
            _state = state;
        }

        public async IAsyncEnumerable<IInboundMessage> RunAsync()
        {
            await foreach(var message in _channel.Reader.ReadAllAsync())
            {
                if (await message.ValidateSession(_state))
                {
                    if (message is IntroAttemptMessage introAttemptMessage)
                        yield return introAttemptMessage;
                }
            }
        }
    }
}

// UserInput(Basic Validation) -> UserInputController(UserInputChannel) -> UserInputStream -> SignedAddressMessageProcessor ( Validation, Business Logic ) --> WebOutputStream --> SignalR Message
//                                                                                                                    --> DiscordOutputStream --> Discord DM