using Apocryph.Dao.Bot.Core.Data;
using Apocryph.Dao.Bot.Core.Streams;
using Nethereum.Parity;
using Nethereum.Signer;
using Newtonsoft.Json.Linq;
using Perper.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Calls
{
    public class Introduce
    {
        private readonly IContext context;

        public Introduce(IContext context) => this.context = context;

        public Task<string> RunAsync()
        {
            var config = JObject.Parse(File.ReadAllText("apocryph-dao.json"));

            var web3 = new Web3Parity((string)config["networkUrl"]);

            var message = "My email is john@doe.com - 1537836206101";
            var signature1 = "<signature_from_frontend>";

            var signer1 = new EthereumMessageSigner();

            var addressRec1 = signer1.EncodeUTF8AndEcRecover(message, signature1);

            return Task.FromResult(addressRec1);
        }
    }
}
