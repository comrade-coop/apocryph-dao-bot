using Nethereum.Parity;
using Newtonsoft.Json.Linq;
using System.IO;

var config = JObject.Parse(File.ReadAllText("apocryph-dao.json"));
var web3 = new Web3Parity((string)config["networkUrl"]);

await Perper.Application.PerperStartup.RunAsync($"apocryph-dao-bot-{System.Guid.NewGuid()}", new System.Threading.CancellationToken());