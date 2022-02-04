using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Ipfs.CoreApi;
using Ipfs.Http;
using Newtonsoft.Json;
using Serilog;

namespace Apocryph.Dao.Bot.Controllers
{
    public class ProposalCreateInput
    {
        public string ContractAddress { get; set; }
        public long ExpirationBlock { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionsHash { get; set; }
    }
    
    [ApiController]
    [Route("api/ipfs/proposal")]
    [Produces("application/json")]
    public class IpfsProposalController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProposalCreateInput input)
        {
            var ipfsClient = new IpfsClient();
            var json = JsonConvert.SerializeObject(input);
            var cid = await ipfsClient.FileSystem.AddTextAsync(json, new AddFileOptions
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
    
    [ApiController]
    [Route("api/webinput")]
    [Produces("application/json")]
    public class WebInputController : Controller
    {
        private readonly Channel<IWebInboundMessage> _channel;

        public WebInputController(Channel<IWebInboundMessage> channel)
        {
            _channel = channel;
        }

        [HttpPost("intro-attempt")]
        public async Task<IActionResult> Post([FromBody] IntroAttemptMessage input) => await HandleWebInboundMessage(input);
 
        private async Task<IActionResult> HandleWebInboundMessage(IWebInboundMessage input)
        {
            var sessionLog = Log.ForContext("Session", input.Session);
            
            await _channel.Writer.WriteAsync(input);
            
            sessionLog.Information("Accepted request {@Input}", input);
            
            return Accepted(string.Empty);
        }
    }
}
