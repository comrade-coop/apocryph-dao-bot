using Apocryph.Dao.Bot.Core.Message;
using Apocryph.Dao.Bot.Core.Streams;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Calls
{
    public class Init
    {
        private readonly IContext _context;

        public Init(IContext context) => _context = context;

        public async Task RunAsync()
        {
            // producer stream
            var discordInputStream = await _context.StreamFunctionAsync<IInboundMessage>(nameof(DiscordInputStream), Array.Empty<object>());
            
            // dialog streams
            var introInquiryDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroInquiryDialogStream), new object[] { discordInputStream });
            var introAttemptDialogStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(IntroAttemptDialogStream), new object[] { discordInputStream });
            
            // event streams
            var transferEventsStream = await _context.StreamFunctionAsync<IOutboundMessage>(nameof(TransferEventsStream), Array.Empty<object>());

            // consumer?
            await _context.StreamActionAsync(nameof(DiscordOutputStream),
                new IStream[] { 
                    introInquiryDialogStream, 
                    introAttemptDialogStream,
                    transferEventsStream });
        }
    }
}