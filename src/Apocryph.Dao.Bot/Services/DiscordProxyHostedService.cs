using System;
using System.Collections.Generic;
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
        private readonly Dictionary<ulong, ISocketMessageChannel> _privateChannels;
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

            _privateChannels = new Dictionary<ulong, ISocketMessageChannel>();
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
            if (!tokens.Any()) return;

            _privateChannels[message.Author.Id] = message.Channel;

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

            if (tokens.Length == 3 && tokens[0] == "/vote" && tokens[1] == "enact" && int.TryParse(tokens[2], out var voteId))
            {
                // TODO: here
                // first issue comes here... we have vote id, but we dont have its ipfs hash :)

                await _inboundChannel.Writer.WriteAsync(new GetBalanceMessage(message.Author.Id));
            }

            if (tokens.Length == 1 && tokens[0] == "/vote")
            {
                // Respond with vote creation URL

                var embedMessage = new EmbedBuilder
                {
                    Title = $"DAO - Vote Creation",
                    Description = "Post a new vote",
                    Url = _config.VoteCreationUrl,
                    ThumbnailUrl = MessageResources.GetRoboHashUrl,
                    Color = new Color(33)
                }.Build();

                // TODO: probably use VoteCreationMessage
                await message.Channel.SendMessageAsync(string.Empty, false, embedMessage);
            }

        }

        private void InitializeMessageSender(CancellationToken cancellationToken)
        {
            _messageSender = Task.Factory.StartNew(async () =>
            {
                await foreach (var message in _outboundChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        if (message is EnactionEventMessage enactionEventMessage)
                        {
                            if (enactionEventMessage.Channel != null)
                            {
                                var channelId = _client.Guilds.First().Channels.Single(x => x.Name == enactionEventMessage.Channel).Id;
                                var channel = _client.GetChannel(channelId) as IMessageChannel;

                                var colorNumber = new Random().Next(0, 16777215);
                                var embedMessage = new EmbedBuilder
                                {
                                    Title = $"Vote proposal: {enactionEventMessage.Title} has been enacted",
                                    Url = enactionEventMessage.GetUrl(),
                                    ThumbnailUrl = enactionEventMessage.GetThumbnailUrl(),
                                    Color = new Color((uint)colorNumber)
                                }.Build();

                                await channel.SendMessageAsync("", false, embedMessage);
                            }
                        }

                        if (message is ProposalEventMessage proposalEventMessage)
                        {
                            if (proposalEventMessage.Channel != null)
                            {
                                var channelId = _client.Guilds.First().Channels.Single(x => x.Name == proposalEventMessage.Channel).Id;
                                var channel = _client.GetChannel(channelId) as IMessageChannel;

                                var colorNumber = new Random().Next(0, 16777215);
                                var embedMessage = new EmbedBuilder
                                {
                                    Title = $"Vote proposal: {proposalEventMessage.Title}",
                                    Description = proposalEventMessage.Description,
                                    Url = proposalEventMessage.GetUrl(),
                                    ThumbnailUrl = proposalEventMessage.GetThumbnailUrl(),
                                    Color = new Color((uint)colorNumber)
                                }.Build();

                                await channel.SendMessageAsync("", false, embedMessage);
                            }
                        }
                        else
                        {
                            await _privateChannels[message.UserId].SendMessageAsync(message.DisplayOutput());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to send discord message");
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
