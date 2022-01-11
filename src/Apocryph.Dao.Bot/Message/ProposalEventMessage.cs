using System.Linq;
using Ipfs;
using Ipfs.Http;
using Newtonsoft.Json;
using Serilog;

namespace Apocryph.Dao.Bot.Message
{
    public record ProposalEventMessage(ulong VoteId, string Rationale, string UrlTemplate) : IOutboundMessage
    {
        public ulong UserId { get; init; }
        public string[] Errors { get; init; }
        public string ContractAddress { get; set; }
        private string Cid { get; set; }
        private string Title { get; set; }
        
        public void FetchData()
        {
            try
            {
                var rationale = Rationale;
                if (rationale.StartsWith("0x"))
                    rationale = rationale.Substring(2);
            
                var base32 = HexString.Decode(rationale);
                var base34 = new byte[] { 0x12, 0x20 }.Concat(base32).ToArray();
                Cid = Base58.Encode(base34);
                
                var client = new IpfsClient();
                
                var data = client.FileSystem.ReadAllTextAsync(Cid).GetAwaiter().GetResult();
                var json = JsonConvert.DeserializeObject<dynamic>(data);
                
                Title = json.title;
                ContractAddress = json.contractAddress;
            }
            catch
            {
                Log.Error("Failed to fetch data for message, {@ProposalEventMessage}", this);
            }
        }
        
        public string DisplayOutput()
        {
            if (!Errors.Any())
            {
                try
                {
                    var url = string.Format(UrlTemplate, Cid);
                
                    return string.Format(MessageResources.ProposalEventMessage_OnSuccess, VoteId, Title, url);
                }
                catch
                {
                    Log.Error("Failed to format message, {@ProposalEventMessage}", this);
                    return $"Vote Proposal VoteId, (ipfs hash {Rationale})";
                }
            }
            
            return string.Format(MessageResources.GenericMessage_OnFailure, ((IOutboundMessage)this).ErrorsStr);
        }
    }
}