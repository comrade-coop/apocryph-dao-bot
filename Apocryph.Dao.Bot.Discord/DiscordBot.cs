using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Apocryph.Dao.Bot.Discord
{
	/// <summary>
	/// This is where the magic happens
	/// </summary>
	public class DiscordBot
	{
		/// <summary>
		/// Another process can send inbound communication to the bot instance.
		/// Inbound communications are then queued and processed.
		/// </summary>
		public class InboundCommunication
		{
			public string plainTextMessage;
			//Note: feel free to add other inbound params here
		}


		private DiscordSocketClient _client;
		private LocalFiles _localFiles;

		private Action<string> _onLogEvent;
		private volatile ConcurrentQueue<InboundCommunication> _inboundQueue = new ConcurrentQueue<InboundCommunication>();
		private bool running => ! _cancellationToken.IsCancellationRequested;
		private CancellationToken _cancellationToken;
		private System.Runtime.CompilerServices.TaskAwaiter mainTask;


		/// <summary>
		/// Creates and runs a new Discord Bot instance
		/// </summary>
		public static DiscordBot CreateInstance(CancellationToken cancellation)
		{
			DiscordBot instance = new DiscordBot() { _cancellationToken = cancellation };
			// We like async ops a lot
			instance.mainTask = instance.MainAsync().GetAwaiter();
			return instance;
		}

		public void Wait()
		{
			mainTask.GetResult();
		}

		public void Push(InboundCommunication input)
		{
			_inboundQueue.Enqueue(input);
		}

		/// <summary>
		/// Here go delegates that listen for discord bot's log output.
		/// (Outbound communication)
		/// </summary>
		public void AddLogEventListener(Action<string> onLogEvent)
		{
			_onLogEvent -= onLogEvent;
			_onLogEvent += onLogEvent;
		}

		private async Task MainAsync()
		{
			// 
			// NOTE (from https://docs.stillu.cc/guides/getting_started/first-bot.html):
			// If your application throws any exceptions within an async context, they will be thrown all the way back up to the first non-async method;
			// since our first non-async method is the program's Main method, this means that all unhandled exceptions will be thrown up there,
			// which will crash your application.
			// Discord.Net will prevent exceptions in event handlers from crashing your program, but any exceptions in your async main will cause the application to crash.
			// 

			// Async context
			Log("Program starting.");

			var config = new DiscordSocketConfig();
			_localFiles = new LocalFiles();
			//TODO: setup config, get discord output channel from file
			const ulong discordChannel_botCommands = 888705313807675432;

			if (_localFiles.LocalFilesReady)
			{
				using (var client = new DiscordSocketClient(config))
				{
					_client = client;
					client.Log += LogDiscordMessage;

					//  You can assign your bot token to a string, and pass that in to connect.
					//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
					var token = _localFiles.DiscordBotToken;

					await client.LoginAsync(TokenType.Bot, token);
					await client.StartAsync();

					//TODO: implement network commands, add in netherium hooks
					client.MessageReceived += MessageReceivedAsync;

					while (running)
					{
						while(!_inboundQueue.IsEmpty)
						{
							InboundCommunication com = null;
							if (_inboundQueue.TryDequeue(out com))
							{
								var channel = client.GetChannel(discordChannel_botCommands);
								if (channel is IMessageChannel messageChannel)
								{
									await messageChannel.SendMessageAsync(com.plainTextMessage);
								}
							}
						}
					}

					Log("Program finished.");
					_client = null;
				}

				_localFiles.SaveFileContents();
			}
			else
			{
				//TODO: More descriptive local files requirements
				Log($"Initialize local files first = {(int) _localFiles.GetRequiredInitalizations()}");
			}
		}

		private async Task MessageReceivedAsync(SocketMessage message)
		{
			//This ensures we don't loop things by responding to ourselves (as the bot)
			if (message.Author.Id == _client?.CurrentUser.Id)
				return;

			// ---------------------- DEBUG --------------------------------
			// NOTE: THE FOLLOWING CODE IS FOR DEBUG PURPOSES ONLY!
#if false
			if (message.Content == ".HelloApocryph")
			{
				var bal = 100;
				if (_localFiles.FakeTokenState.Add(message.Author.Username, bal, out string id))
				{
					Log($"Added user {message.Author.Username}. ID is {id}.");
					await message.Channel.SendMessageAsync($"Hello {message.Author.Username}. Your user ID is {id}. Use that ID for transfers. You have been awarded {bal} FAKE tokens.");
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
				foreach (var user in _localFiles.FakeTokenState.data)
				{
					output.AppendLine($"{user.Key} : {user.Value}");
				}
				output.AppendLine("----- ---------------- -----");

				Log($"[{message.Author.Username}] Requested the state of the token.");
				await message.Channel.SendMessageAsync(output.ToString());
			}

			if (message.Content.StartsWith(".Help"))
			{
				var commandsList = new StringBuilder();
				commandsList.AppendLine("---- Apocryph DAO Bot FAKE Commands ----");
				commandsList.AppendLine(".HelloApocryph");
				commandsList.AppendLine(".Pay {recipient id} {amount}");
				commandsList.AppendLine(".Bal");
				commandsList.AppendLine(".People");
				commandsList.AppendLine(".Help");
				commandsList.AppendLine(".TestInboundCommunication");
				commandsList.AppendLine("----- ---------------- -----");

				await message.Channel.SendMessageAsync(commandsList.ToString());
			}

			if (message.Content.StartsWith(".Pay"))
			{
				var inputs = message.Content.Substring(4, message.Content.Length - 4).Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (inputs.Length == 2)
				{
					string response;
					_localFiles.FakeTokenState.Pay(message.Author.Username, inputs[0], int.Parse(inputs[1]), out response);
					Log($"[{message.Author.Username}] {response}");
					await message.Channel.SendMessageAsync(response);
				}
				else
				{
					await message.Channel.SendMessageAsync("Wrong number of arguments!");
				}
			}

			if (message.Content.StartsWith(".Bal"))
			{
				bool hasError;
				string errorMsg;
				float bal = _localFiles.FakeTokenState.Bal(message.Author.Username, out hasError, out errorMsg);

				if (hasError)
				{
					Log($"[{message.Author.Username}] {errorMsg}");
					await message.Channel.SendMessageAsync(errorMsg);
				}
				else
				{
					var balMsg = $"Current balance for {message.Author.Username} is {bal}";
					Log($"[{message.Author.Username}] {balMsg}");
					await message.Channel.SendMessageAsync(balMsg);
				}
			}

			if (message.Content.StartsWith(".TestInboundCommunication"))
			{
				Push(new InboundCommunication() { plainTextMessage = "Inbound communications work correctly." });
			}
#endif
			// -----------------------------------------------------

			//TODO: Add token trading commands here
		}

		/// <summary>
		/// Catch log events and print to console. Subscribe to the discord socket client.
		/// </summary>
		private Task LogDiscordMessage(LogMessage msg)
		{
			// Discord.NET uses proprietary LogMessage for error handling
			Log(msg.ToString(), false);
			return Task.CompletedTask;
		}

		private void Log (object msg, bool timeStamp = true)
		{
			var message = timeStamp ? $"[{DateTime.Now}] {msg}" : msg.ToString();
			Console.WriteLine(message);
			_onLogEvent?.Invoke(message);
		}
	}
}
