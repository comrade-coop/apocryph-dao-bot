using Apocryph.Dao.Bot.Streams;
using Perper.Model;
using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;

namespace Apocryph.Dao.Bot.Calls
{
    public class Init
    {
        private readonly IContext _context;

        public Init(IContext context) => _context = context;

        public async Task RunAsync()
        {
            // producers
            var discordInputStream = await _context.StreamFunctionAsync<IInboundMessage>(nameof(DiscordInputStream), Array.Empty<object>()).ConfigureAwait(false);
            var webInputStream = await _context.StreamFunctionAsync<IInboundMessage>(nameof(WebInputStream), Array.Empty<object>()).ConfigureAwait(false);
            
            // processor
            var introInquiryDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroInquiryDialogStream), new object[] { discordInputStream }).ConfigureAwait(false);
            var introAttemptDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroAttemptDialogStream), new object[] { webInputStream }).ConfigureAwait(false);

            await _context.StreamActionAsync(nameof(DiscordOutputStream),
                new object[]
                {
                    introInquiryDialogStream, 
                    introAttemptDialogStream
                });

            await _context.StreamActionAsync(nameof(WebOutputStream), 
                new object[]
                {
                    introAttemptDialogStream
                });
        }
    }
}