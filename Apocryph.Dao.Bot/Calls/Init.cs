using Apocryph.Dao.Bot.Message;
using Apocryph.Dao.Bot.Streams;
using Perper.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Calls
{
    public class Init
    {
        private readonly IContext _context;

        public Init(IContext context) => _context = context;

        public async Task RunAsync()
        {
            // producer
            var discordInputStream = await _context.StreamFunctionAsync<IInboundMessage>(nameof(DiscordInputStream), Array.Empty<object>()).ConfigureAwait(false);
            
            // processor
            var introInquiryDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroInquiryDialogStream), new object[] { discordInputStream }).ConfigureAwait(false);
            
            await _context.StreamActionAsync(nameof(DiscordOutputStream), 
                new IStream[]
                {
                    introInquiryDialogStream
                });

            
            // await _context.StreamActionAsync(nameof(DiscordOutputStream), new IStream[] { discordInputStream });

            // dialog streams
            //var introInquiryDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroInquiryDialogStream), new object[] { discordInputStream });

            // event streams
            //var transferEventsStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(TransferEventsStream), Array.Empty<object>());

            // consumer?
            //await _context.StreamActionAsync(nameof(DiscordOutputStream),
//                new IStream[] { 
//                    introInquiryDialogStream, 
//                    introAttemptDialogStream,
//                    transferEventsStream });
            // discordInputStream.ToListAsync();
        }
    }
}