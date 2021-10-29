using Apocryph.Dao.Bot.Core.Data.Message;
using Apocryph.Dao.Bot.Core.Message;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Parity;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class IntroInquiryDialog
    {
        private readonly IContext context;

        public IntroInquiryDialog(IContext context) => this.context = context;

        public async IAsyncEnumerable<IntroChallangeMessage> RunAsync(IntroInquiryMessage inquiry)
        {
            throw new NotImplementedException();
        }
    }
}
