using Apocryph.Dao.Bot.Core.Data.Message;
using Apocryph.Dao.Bot.Core.Message;
using Perper.Model;
using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class IntroInquiryDialog
    {
        private readonly IContext context;
        private readonly IState state;

        public IntroInquiryDialog(IContext context, IState state)
        {
            this.context = context;
            this.state = state;
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
