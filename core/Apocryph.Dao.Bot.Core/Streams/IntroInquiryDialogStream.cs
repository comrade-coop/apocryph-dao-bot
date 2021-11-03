using Apocryph.Dao.Bot.Core.Data.Message;
using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class IntroInquiryDialogStream
    {
        private readonly IContext _context;
        private readonly IState _state;

        public IntroInquiryDialogStream(IContext context, IState state)
        {
            _context = context;
            _state = state;
        }

        public async IAsyncEnumerable<IntroChallangeMessage> RunAsync(IAsyncEnumerable<IntroInquiryMessage> inquiryMessages)
        {
            await foreach(var inquiry in inquiryMessages)
            {
                yield return new IntroChallangeMessage();
            }
        }
    }
}
