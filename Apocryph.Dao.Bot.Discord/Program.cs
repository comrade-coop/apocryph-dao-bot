using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Apocryph.Dao.Bot.Discord
{
	class Program
	{
		private static DiscordSocketClient _client;
		private static LocalFiles _localFiles;

		public static void Main(string[] args)
		{
			// We like async ops a lot
			new Program().MainAsync().GetAwaiter().GetResult();
		}

		public async Task MainAsync()
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
			//TODO: setup config
			_localFiles = new LocalFiles();

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

					// Block this task until the program is closed.
					await Task.Delay(-1);
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
			Console.WriteLine(timeStamp ? $"[{DateTime.Now}] {msg}" : msg.ToString());
		}
	}
}
