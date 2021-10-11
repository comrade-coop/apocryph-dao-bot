using Apocryph.Dao.Bot.Core.Data;
using Nethereum.Parity;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Core.Streams
{
    public class TransferEvents
    {
        private readonly IContext context;

        public TransferEvents(IContext context) => this.context = context;

        public async IAsyncEnumerable<TransferEventDTO> RunAsync()
        {
            var config = JObject.Parse(File.ReadAllText("apocryph-dao.json"));
            
            var web3 = new Web3Parity((string)config["networkUrl"]);
            var transferEventHandler = web3.Eth.GetEvent<TransferEventDTO>((string)config["contracts"]["bondingCurveToken"]["address"]);
            var transferFilterInput = transferEventHandler.CreateFilterInput(new BlockParameter(ulong.Parse((string)config["startBlock"])), BlockParameter.CreateLatest());
            var allChanges = await transferEventHandler.GetAllChangesAsync(transferFilterInput);
            foreach (var change in allChanges)
            {
                yield return change.Event;
            }

            var transferFilterInputId = await transferEventHandler.CreateFilterAsync(transferFilterInput);
            while (true)
            {
                var changes = await transferEventHandler.GetFilterChangesAsync(transferFilterInputId);
                foreach (var change in changes)
                {
                    yield return change.Event;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
