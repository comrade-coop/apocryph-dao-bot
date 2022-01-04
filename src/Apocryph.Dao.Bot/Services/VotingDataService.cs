using System;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Ipfs.CoreApi;
using Ipfs.Http;
using Newtonsoft.Json;
using Serilog;

namespace Apocryph.Dao.Bot.Services
{
    public class VotingDataService
    {
        private readonly IpfsClient _ipfsEngine;

        public VotingDataService(IpfsClient ipfsEngine)
        {
            _ipfsEngine = ipfsEngine;
        }
        
        public async Task<string> Store(IWebInboundMessage message)
        {
            try
            {
                var json = JsonConvert.SerializeObject(message);
                var fsn = await _ipfsEngine.FileSystem.AddTextAsync(json, new AddFileOptions
                {
                    Pin = true
                });
                    
                return fsn.Id.Hash.ToBase58();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to store data for {@Message}", message);
                return null;
            }
        }
    }
}