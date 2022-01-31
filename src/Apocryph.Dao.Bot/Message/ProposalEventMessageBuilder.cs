using System;
using System.Linq;
using Apocryph.Dao.Bot.Configuration;
using Ipfs;
using Ipfs.Http;
using Newtonsoft.Json;
using Serilog;

namespace Apocryph.Dao.Bot.Message
{
    public class ProposalEventMessageBuilder
    {
        public ProposalEventMessage Build(DaoBotConfig config, string contractAddress, ulong voteId, byte[] rationale)
        {
            var base34 = new byte[] { 0x12, 0x20 }.Concat(rationale).ToArray();
            var cid = Base58.Encode(base34);
                
            var message = new ProposalEventMessage(voteId, cid)
            {
                UrlTemplate = config.VoteProposalUrl,
                ContractAddress = contractAddress,
                Channel = "community" // config.GetDaoVotingName(contractAddress)
            };
            
            try
            {
                //var client = new IpfsClient();
                //var data = client.FileSystem.ReadAllTextAsync(cid).GetAwaiter().GetResult();
                //var json = JsonConvert.DeserializeObject<dynamic>(data);
                //message.Title = json.title;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Failed to fetch data for message, {@ProposalEventMessage}", this);
            }

            return message;
        }
    }
}