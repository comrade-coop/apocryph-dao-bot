using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Streams;
using Perper.Model;
// ReSharper disable CoVariantArrayConversion

namespace Apocryph.Dao.Bot.Calls
{
    public class UserInteractions
    {
        private readonly IContext _context;
        
        public UserInteractions(IContext context)
        {
            _context = context;
        }

        public async Task RunAsync()
        {
            var discordInputStream = await _context.StreamFunctionAsync<IInboundMessage>(nameof(DiscordInputStream), Array.Empty<object>()).ConfigureAwait(false);
            var webInputStream = await _context.StreamFunctionAsync<IWebInboundMessage>(nameof(WebInputStream), Array.Empty<object>()).ConfigureAwait(false);

            var introInquiryDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroInquiryStream), new object[] { discordInputStream }).ConfigureAwait(false);
            var getBalanceDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(GetBalanceStream), new object[] { discordInputStream }).ConfigureAwait(false);
            var airdropTentUserStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(AirdropTentUserStream), new object[] { discordInputStream }).ConfigureAwait(false);
            var introAttemptDialogStream = await _context.StreamFunctionAsync<IWebOutboundMessage>(nameof(IntroAttemptStream), new object[] { webInputStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { introInquiryDialogStream });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { getBalanceDialogStream });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { airdropTentUserStream });
            await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { introAttemptDialogStream });
            await _context.StreamActionAsync(nameof(WebOutputStream), new IStream[] { introAttemptDialogStream });
        }
    }
}