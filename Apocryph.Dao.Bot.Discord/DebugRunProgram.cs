namespace Apocryph.Dao.Bot.Discord
{
	public static class RunInDebug
	{
		/// <summary>
		/// This exists for debugging purposes (running the bot through debug mode in VS)
		/// </summary>
		public static void Main()
		{
			DiscordBot.CreateInstance(new System.Threading.CancellationToken());
		}
	}
}