using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Apocryph.Dao.Bot.Discord
{
	class Program
	{
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
			var localFiles = new LocalFiles();

			if (localFiles.LocalFilesReady)
			{
				using (var client = new DiscordSocketClient(config))
				{
					client.Log += LogDiscordMessage;

					//  You can assign your bot token to a string, and pass that in to connect.
					//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
					var token = localFiles.DiscordBotToken;

					await client.LoginAsync(TokenType.Bot, token);
					await client.StartAsync();

					//TODO: implement network commands, add in netherium hooks

					// Block this task until the program is closed.
					await Task.Delay(-1);
					Log("Program finished.");
				}
			}
			else
			{
				//TODO: More descriptive local files requirements
				Log($"Initialize local files first = {(int) localFiles.GetRequiredInitalizations()}");
			}
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
