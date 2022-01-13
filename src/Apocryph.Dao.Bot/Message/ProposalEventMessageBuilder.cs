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
        public ProposalEventMessage Build(DaoBotConfig config, ulong voteId, string rationale)
        {
            var message = new ProposalEventMessage(voteId, rationale)
            {
                UrlTemplate = config.VoteProposalUrl
            };

            try
            {
                if (rationale.StartsWith("0x"))
                    rationale = rationale.Substring(2);
            
                var base32 = HexString.Decode(rationale);
                var base34 = new byte[] { 0x12, 0x20 }.Concat(base32).ToArray();
                var cid = Base58.Encode(base34);
                
                var client = new IpfsClient();
                var data = client.FileSystem.ReadAllTextAsync(cid).GetAwaiter().GetResult();
                var json = JsonConvert.DeserializeObject<dynamic>(data);
                message.Title = json.title;
                message.ContractAddress = json.contractAddress;
                message.DaoName = config.GetDaoVotingName(message.ContractAddress);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Failed to fetch data for message, {@ProposalEventMessage}", this);
            }

            return message;
        }
    }
}