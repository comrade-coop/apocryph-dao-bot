﻿using Apocryph.Dao.Bot.Core.Data.Message;
using Nethereum.Parity;
using Nethereum.Signer;
using Nethereum.Web3;
using Perper.Model;
using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class IntroAttemptDialog
    {
        private readonly IContext context;
        private readonly IWeb3 web3;

        public IntroAttemptDialog(IContext context, IWeb3 web3)
        {
            this.context = context;
            this.web3 = web3;
        }

        public async IAsyncEnumerable<IntroConfirmationMessage> RunAsync(IntroAttemptMessage attempt)
        {
            var message = "My email is john@doe.com - 1537836206101";
            var signature1 = "0x200db785c1b4f2bcf03250fc14b31b09299ed6801f43d64438b07ef38eb2b1ab370f110bc39ce27bd72fcc3cd39bb3812d4e2dbe47f55a46492fd5f2370df0bd1c";

            var signer1 = new EthereumMessageSigner();

            var addressRec1 = signer1.EncodeUTF8AndEcRecover(message, signature1);

            throw new NotImplementedException();
        }
    }
}
