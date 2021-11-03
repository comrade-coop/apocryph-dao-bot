using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Core.Message;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using Apocryph.Dao.Bot.Core.Model;

namespace Apocryph.Dao.Bot.Core.Services
{
    public sealed class MessageListener : IHostedService
    {
        private readonly IOptions<Configuration.Discord> _options;
        private readonly LocalTokenState _localTokenState;
        private readonly Channel<IInboundMessage> _channel;
        private readonly DiscordSocketConfig _socketConfig;
        private DiscordSocketClient _client;

        private LocalToken _localToken;

        public MessageListener(IOptions<Configuration.Discord> options, DiscordSocketConfig socketConfig, LocalTokenState localTokenState, Channel<IInboundMessage> channel)
        {
            _options = options;
            _localTokenState = localTokenState;
            _channel = channel;
            _socketConfig = socketConfig;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _localTokenState.Initialize(out _localToken);

            _client = new DiscordSocketClient(_socketConfig);
            _client.Log += async message =>
            {
                if (message.Exception != null)
                {
                    Log.Error(message.Exception, "Failed to process discord message");
                }
                else
                {
                    Log.Information("{Message}", message.Message);
                }

                await Task.CompletedTask;
            };

            _client.MessageReceived += MessageReceivedAsync;

            await _client.LoginAsync(TokenType.Bot, _options.Value.AuthToken);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _localTokenState.Store(_localToken);

            await Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _client?.CurrentUser.Id)
                return;

            await _channel.Writer.WriteAsync(new IntroAttemptMessage());
            
            return;
            // ---------------------- DEBUG --------------------------------
            // NOTE: THE FOLLOWING CODE IS FOR DEBUG PURPOSES ONLY!

            if (message.Content == ".HelloApocryph")
            {
                var balance = 100m;

                var addResult = _localToken.Add(message.Author.Username, message.Author.Id, balance);
                if (addResult.IsValid)
                {
                    Log.Information("Added user {Username}; ID is {Id}", message.Author.Username, message.Author.Id);

                    await message.Channel.SendMessageAsync($"Hello {message.Author.Username}. Your user ID is {message.Author.Id}. Use that ID for transfers. You have been awarded {balance} FAKE tokens.");
                }
                else
                {
                    await message.Channel.SendMessageAsync($"Welcome back {message.Author.Username}.");
                }
            }

            if (message.Content.StartsWith(".People"))
            {
                var output = new StringBuilder();
                output.AppendLine("----- FAKE TOKEN STATE -----");
                foreach (var user in _localToken.UserBalances)
                {
                    output.AppendLine($"{user.Key} : {user.Value}");
                }
                output.AppendLine("----- ---------------- -----");

                Log.Information("[{Username}] Requested the state of the token", message.Author.Username);

                await message.Channel.SendMessageAsync(output.ToString());
            }

            if (message.Content.StartsWith(".Help"))
            {
                var commandsList = new StringBuilder();
                commandsList.AppendLine("---- Apocryph DAO Bot FAKE Commands ----");
                commandsList.AppendLine(".HelloApocryph");
                commandsList.AppendLine(".Pay @mention {amount}");
                commandsList.AppendLine(".Balance");
                commandsList.AppendLine(".People");
                commandsList.AppendLine(".Help");
                commandsList.AppendLine("----- ---------------- -----");

                await message.Channel.SendMessageAsync(commandsList.ToString());
            }

            if (message.Content.StartsWith(".Pay"))
            {
                if (message.MentionedUsers.Count == 1)
                {
                    if (message.MentionedUsers.Single().Id != message.Author.Id)
                    {
                        var inputs = message.Content.Substring(4, message.Content.Length - 4).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (inputs.Length == 2)
                        {
                            var amount = decimal.Parse(inputs[1], System.Globalization.NumberStyles.Any);
                            var payResult = _localToken.Pay(message.Author.Username, message.MentionedUsers.Single().Username, amount);

                            Log.Information("[{Username}] {Message}", message.Author.Username, payResult.Message);

                            await message.Channel.SendMessageAsync(payResult.Message);
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("Wrong number of arguments!");
                        }
                    }
                }
            }

            if (message.Content.StartsWith(".Balance"))
            {
                var balanceResult = _localToken.Balance(message.Author.Username);

                if (!balanceResult.IsValid)
                {
                    Log.Information("[{Username}] {Message}", message.Author.Username, balanceResult.Message);

                    await message.Channel.SendMessageAsync(balanceResult.Message);
                }
                else
                {
                    var balanceMessage = $"Current balance for {message.Author.Username} is {balanceResult.Amount} FAKE";
                    Log.Information("[{Username}] {Message}", message.Author.Username, balanceMessage);
                    await message.Channel.SendMessageAsync(balanceMessage);
                }
            }
            // -----------------------------------------------------

            //TODO: Add token trading commands here
        }
    }
}
