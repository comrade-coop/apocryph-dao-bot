using System.IO;
using Microsoft.Extensions.Configuration;

namespace Apocryph.Dao.Bot.Tests
{
    public static class SharedConfigurationExtensions
    {
        public static IConfigurationBuilder ConfigureTestSettings(this IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, false);
            builder.AddJsonFile("appsettings.Development.json", false, false);
            builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
            return builder;
        }
    }
}