using System.Linq;

namespace Apocryph.Dao.Bot
{
    public static class DiscordExtensions
    {
        public static string GetSlashCommand(this string message)
        {
            return message.StartsWith("/") ? message.Split(" ").First()[1..] : string.Empty;
        }
    }
}
