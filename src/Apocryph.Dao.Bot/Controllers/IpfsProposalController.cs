using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Models;
using Ipfs.CoreApi;
using Ipfs.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Apocryph.Dao.Bot.Controllers
{
    [ApiController]
    [Route("api/ipfs/proposal")]
    [Produces("application/json")]
    public class IpfsProposalController : Controller
    {
        private readonly IpfsClient IpfsClient = new IpfsClient();

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string cid)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10 * 1000);

            await using var stream = IpfsClient.PostDownloadAsync("cat", cancellationTokenSource.Token, cid).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            
            var json = JsonConvert.DeserializeObject<VoteProposal>(data);

            return Ok(json);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VoteProposal input)
        {
          
            var json = JsonConvert.SerializeObject(input);
            var cid = await IpfsClient.FileSystem.AddTextAsync(json, new AddFileOptions
            {
                Pin = true
            });

            return Created(string.Empty, new
            {
                Cid = cid.Id.ToString()
            });
           
            
            
            // TODO: schedule document for deletion in next 3 minutes
        }
        
        // confirm document to smart contract assigment
        // remove deletion task
    }
}