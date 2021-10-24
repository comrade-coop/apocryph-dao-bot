using Apocryph.Dao.Bot.Core.Data;
using Apocryph.Dao.Bot.Core.Streams;
using Perper.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Calls
{
	public class Init
	{
		private readonly IContext context;

		public Init(IContext context) => this.context = context;

		public async Task RunAsync()
		{
			var address = await context.CallFunctionAsync<string>(nameof(Introduce), Array.Empty<object>());
			var transferEvents = await context.StreamFunctionAsync<TransferEventDTO>(nameof(TransferEvents), Array.Empty<object>());

			var botCancellation = new CancellationToken();
			var discordBotInstance = Apocryph.Dao.Bot.Discord.DiscordBot.CreateInstance(botCancellation);
			await foreach (var transferEvent in transferEvents)
			{
				var message = $"Ingested: event Transfer(from: {transferEvent.From}, to: {transferEvent.To}, value: {transferEvent.Value})";
				//Console.WriteLine();
				discordBotInstance.Push(new Discord.DiscordBot.InboundCommunication() { plainTextMessage = message });
			}

			//TODO: cancellation and await discordBotInstance.Dispose();
		}
	}
}
