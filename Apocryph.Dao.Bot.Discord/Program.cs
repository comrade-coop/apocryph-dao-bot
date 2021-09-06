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

			var config = new DiscordSocketConfig();
			//TODO: setup config

			using (var client = new DiscordSocketClient(config))
			{
				client.Log += Log;

				//TODO: subscribe stuff here...
				//TODO: listening loop?
			}
		}

		/// <summary>
		/// Catch log events and print to console. Subscribe to the discord bot client.
		/// </summary>
		private Task Log(LogMessage msg)
		{
			// Discord.NET uses proprietary LogMessage for error handling
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
