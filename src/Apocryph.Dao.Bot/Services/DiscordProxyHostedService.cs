using System;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;

namespace Apocryph.Dao.Bot.Services
{
    public sealed class DiscordProxyHostedService : IHostedService
    {
        private readonly Configuration.DaoBotConfig _config;
        private readonly DiscordSocketConfig _socketConfig;
        private DiscordSocketClient _client;
        private readonly Channel<IInboundMessage> _inboundChannel;
        private readonly Channel<IOutboundMessage> _outboundChannel;
        private Task _messageSender;

        public DiscordProxyHostedService(
            IOptions<Configuration.DaoBotConfig> options,
            DiscordSocketConfig socketConfig,
            Channel<IInboundMessage> inboundChannel, 
            Channel<IOutboundMessage> outboundChannel)
        {
            _config = options.Value;
            _socketConfig = socketConfig;
            _inboundChannel = inboundChannel;
            _outboundChannel = outboundChannel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new DiscordSocketClient(_socketConfig);
            _client.MessageReceived += MessageReceivedAsync;
            _client.Log += OnClientOnLog;

            await _client.LoginAsync(TokenType.Bot, _config.DiscordAuthToken);
            await _client.StartAsync();

            InitializeMessageSender(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _messageSender.Dispose();
            
            await Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _client?.CurrentUser.Id)
                return;

            Log.Information("Receive message: {Content}", message.Content);

            var tokens = message.Content.Split(' ');
            if(!tokens.Any())return;
            
            if (tokens[0] == "/introduce")
            {
                var address = tokens[1];
                await _inboundChannel.Writer.WriteAsync(new IntroInquiryMessage(message.Author.Username, message.Author.Id, address));
                return;
            }
            
            if (tokens[0] == "/airdrop" && tokens[1] == "tent")
            {
                var userExistInTentServer = message.Author.MutualGuilds
                    .Where(x => x.Name == "TENT")
                    .SelectMany(x => x.Users)
                    .Any(x => x.Id == message.Author.Id);
                
                await _inboundChannel.Writer.WriteAsync(new AirdropTentUserMessage(message.Author.Id, userExistInTentServer));
                return;
            }
            
            if (tokens[0] == "/balance")
            {
                await _inboundChannel.Writer.WriteAsync(new GetBalanceMessage(message.Author.Id));
            }
            
          
            
            // ---------------------- DEBUG --------------------------------
            // NOTE: THE FOLLOWING CODE IS FOR DEBUG PURPOSES ONLY!
            /*
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
*/
            
            //TODO: Add token trading commands here
        }
        
        private void InitializeMessageSender(CancellationToken cancellationToken)
        {
            _messageSender = Task.Factory.StartNew(async () =>
                {
                    await foreach (var message in _outboundChannel.Reader.ReadAllAsync(cancellationToken))
                    {
                        if (message is ProposalEventMessage proposalEventMessage)
                        {
                            if (proposalEventMessage.DaoName != null)
                            {
                                var channelGroup = _client.GroupChannels.FirstOrDefault(x => x.Name.Equals(proposalEventMessage.DaoName, StringComparison.CurrentCultureIgnoreCase));
                                var channel = await _client.GetGroupChannelAsync(channelGroup.Id);
                                await channel.SendMessageAsync(proposalEventMessage.DisplayOutput());
                            }
                        }
                        else
                        {
                            await _client
                                .GetUser(message.UserId)
                                .SendMessageAsync(message.DisplayOutput());    
                        }
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private async Task OnClientOnLog(LogMessage message)
        {
            if (message.Exception != null)
            {
                Log.Error(message.Exception, "Failed to process discord message");
            }
            else
            {
                Log.Information("MessageListener: {Message}", message.Message);
            }

            await Task.CompletedTask;
        }
    }
}
