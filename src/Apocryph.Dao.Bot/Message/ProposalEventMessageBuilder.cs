using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Configuration;
using Apocryph.Dao.Bot.Events;
using Apocryph.Dao.Bot.Infrastructure;
using Ipfs;
using Ipfs.Http;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace Apocryph.Dao.Bot.Message
{
    public class ProposalEventMessageBuilder
    {
        public ProposalEventMessage Build(DaoBotConfig config, EventLog<ProposalEventDTO> eventLog)
        {
            var voteId =  eventLog.Event.VoteId.ToHex(true);
            var base34 = new byte[] { 0x12, 0x20 }.Concat(eventLog.Event.Rationale).ToArray();
            var cid = Base58.Encode(base34);
            
            var message = new ProposalEventMessage(voteId, cid)
            {
                UrlTemplate = config.VoteProposalUrl,
                ContractAddress = eventLog.Log.Address,
                Channel = config.DaoVotingAddresses.Where(x => x.Value.ToLower() == eventLog.Log.Address).Select(x => x.Key).FirstOrDefault(),
                Title = "New Voting proposal",
                Description = string.Empty
            };

            try
            {
                var client = new IpfsClient();
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(10 * 1000);

                using var stream = client.PostDownloadAsync("cat", cancellationTokenSource.Token, cid)
                    .GetAwaiter()
                    .GetResult();
                
                using var reader = new StreamReader(stream);
                var data = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject<dynamic>(data);

                message.Title = json.Title;
                message.Description = StringHelper.Truncate(json.Description.ToString(), 256, "...");
            }
            catch (TaskCanceledException)
            {
                Log.Warning("Failed to fetch data for message, {@EventLog}", eventLog);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Failed to build message, {@EventLog}", eventLog);
            }

            return message;
        }
    }
}